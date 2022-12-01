using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform bulletProjectile;
    [SerializeField] private Transform bulletSpawnPosision;


    private enum ShootType
    {
        AUTOMATIC,
        BURST,
        SINGLEFIRE
    }

    [Header("Shooting")]

    [SerializeField] private ShootType shootingType;
    [SerializeField] private float fireRate;
    [Range(0, 1)]
    [SerializeField] private float accuracy;
    [Tooltip("Percentage of accuracy when shooting without aiming.")]
    [Range(0, 1)]
    [SerializeField] private float hipfireAccuracy;
    [Range(1, 20)]
    [SerializeField] private int numberOfbullets;

    private Vector3 aimPoint;
    private float aimRotationSpeed = 20f;

    private bool canShoot = true;

    private float currentAccuracy;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit hit, 999f, aimColliderLayerMask))
        {
            aimPoint = hit.point;
        }
        
        Vector3 worldAimTarget = aimPoint;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, aimRotationSpeed * Time.deltaTime);

            currentAccuracy = accuracy;
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);

            currentAccuracy = accuracy * hipfireAccuracy;
        }

        if (starterAssetsInputs.shoot)
        {
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, aimRotationSpeed * Time.deltaTime);

            thirdPersonController.SetRotateOnMove(false);

            if (!canShoot) { return; }

            Vector3 aimDir = (aimPoint - bulletSpawnPosision.position).normalized;

            float spread = (15 - (15 * currentAccuracy)) * Mathf.Deg2Rad;

            switch (shootingType)
            {
                case ShootType.AUTOMATIC:
                    StartCoroutine(ShootWait(1 / fireRate));
                    break;
                case ShootType.BURST:
                    break;
                case ShootType.SINGLEFIRE:
                    StartCoroutine(ShootWait(1 / fireRate));
                    starterAssetsInputs.shoot = false;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < numberOfbullets; i++)
            {
                Vector3 shootDir = aimDir;

                shootDir.x += Random.Range(-spread, spread);
                shootDir.y += Random.Range(-spread, spread);
                shootDir.z += Random.Range(-spread, spread);

                Instantiate(bulletProjectile, bulletSpawnPosision.position, Quaternion.LookRotation(shootDir, Vector3.up));
            }

            if(fireRate > 0)
            {
                StartCoroutine(ShootWait(1 / fireRate));
            }
            else
            {
                starterAssetsInputs.shoot = false;
            }
        }
        else
        {
            thirdPersonController.SetRotateOnMove(true);
        }
    }

    IEnumerator ShootWait(float waitTime)
    {
        canShoot = false;

        yield return new WaitForSeconds(waitTime);

        canShoot = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(aimPoint, new Vector3(0.5f, 0.5f, 0.5f));
    }
}
