/*
	Задача.
	Создать сцену, на которой будет размещенно N кубов. 
	Камера должна плавно перемещаться от одного куба к другому путем нажатия на соответствующие кнопки. 
	Реализовать возможность изменения цвета просматриваемого куба.
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamControl : MonoBehaviour {

	public GameObject[] cubes; //Массив, в котором будут храниться кубы

	Button RightButton; //Кнопки навигации
	Button LeftButton;


	public GameObject target; //Куб, на который обращен взор камеры
	int counter; //Номер куба, на который смотрит камера

	Vector3 currPos; //Текущая позиция камеры
	Vector3 nextPos; //Позиция, куда полетит камера 

	Transform leftLimit; //Левая граница
	Transform rightLimit; //Правая граница

	float len = 20f; //длинна луча
	public float arriveTime = 3f; //Скорость полета камеры


	void Start(){
		counter = 0;
		/*Костыль для обнаружения кнопок.
		  FindObjectOfType'ом находит первую в порядке отрисовки Canvas'a кнопку LefttButton, дизейблим её,
		  FindObjectOfType снова находит кнопку, но уже вторую (RightButton), поскольку LeftButton не активна.
		  Активируем RightButton.
		*/
		LeftButton = FindObjectOfType <Button> ();
		LeftButton.gameObject.SetActive (false);
		RightButton = FindObjectOfType <Button> ();
		LeftButton.gameObject.SetActive (true);

		//Ищем по тэгу кубы на сцене, сортируем и помещаем их в массив 
		cubes = GameObject.FindGameObjectsWithTag("Cube").OrderBy(go => go.transform.position.x).ToArray();

		//Перемещаем камеру на позицию первого куба в массиве cubes
		gameObject.transform.position = new Vector3 (cubes [0].transform.position.x, cubes [0].transform.position.y, cubes [0].transform.position.z - 10f);

	}

	void Update(){
		//Перемещение камеры
		currPos = gameObject.transform.position;
		transform.position = Vector3.Lerp (currPos, new Vector3(nextPos.x, nextPos.y, nextPos.z-10f), Time.deltaTime*arriveTime);

		//Ограничение по правому краю
		if (counter == cubes.Length-1) {
			RightButton.interactable = false;
		} else {
			RightButton.interactable = true;
		}

		//Ограничение по левому краю
		if (counter == 0) {
			LeftButton.interactable = false;
		} else {
			LeftButton.interactable = true;
		}

	}

	//Обнаружение куба с помощью коллизии луча с коллайдером куба
	void FixedUpdate()
	{
		RaycastHit hit;
		var ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2, Screen.height/2));
		Debug.DrawRay(ray.GetPoint(len),ray.direction, Color.green);
		if(Physics.Raycast(ray, out hit, len)){
			target = hit.transform.gameObject;
		}
	}

	//Перемещение камеры вправо
	public void MoveRight(){
		target = null;
		if (counter < cubes.Length-1) {
			counter++;
			nextPos = cubes [counter].transform.position;
		} 

	}

	//Перемещение камеры влево
	public void MoveLeft(){
		target = null;
		if (counter > 0) {
			counter--;
			nextPos = cubes [counter].transform.position;
		} 
	}

	//Изменение цвета куба, на который смотрит камера
	public void ChangeColor(){
		if (target != null) {
			Renderer targetColor = target.GetComponent<Renderer> ();
			targetColor.material.color = UnityEngine.Random.ColorHSV ();
		}
	}
}
