using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class ArcherBehavior : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation; //Spine-анимация
    public GameObject arrowPrefab; //Префаб стрелы
    public Transform arrowSpawnPoint; //Точка спавна стрелы
    public float shootForce = 20f; //Сила выстрела
    private bool isAiming = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //+ЛКМ для начала прицеливания
        {
            StartAiming();
        }

        if (Input.GetMouseButtonUp(0)) //-ЛКМ для выстрела
        {
            Shoot();
        }
        if (isAiming)
        {
            //Получаем направление от персонажа к курсору мыши
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 targetPoint = ray.GetPoint(distance);
                Vector3 direction = (targetPoint - arrowSpawnPoint.position).normalized;

                //Поворачиваем точку спавна стрелы в направлении цели
                arrowSpawnPoint.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    void StartAiming()
    {
        isAiming = true;
        skeletonAnimation.AnimationState.SetAnimation(0, "Aim", true); //Анимация прицела
    }

    void Shoot()
    {
        if (!isAiming) return;

        isAiming = false;
        skeletonAnimation.AnimationState.SetAnimation(0, "Shoot", false); //Анимация выстрела
        skeletonAnimation.AnimationState.AddAnimation(0, "Idle", true, 0); //Стандартная анимация

        //Создание стрелы
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            //Сила полета стрелы
            rb.velocity = arrowSpawnPoint.forward * shootForce;
        }
    }
}
