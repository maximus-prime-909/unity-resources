using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingCameraScript : MonoBehaviour
{
	[Header("TPC")]
	[SerializeField] float mouseSenstivity = 5f;
	[SerializeField] Transform target;

	[SerializeField] Vector2 pitchMinMax = new Vector2(-40, 30);
	[SerializeField] float rotationSmoothTime = 0.12f;

	private Vector3 rotationSmoothVelocity;
	private Vector3 currentRotation;

	private float pitch;
	private float yaw;

	private float origionalMouseSenstivity;	

	[Header("Collision")]
	[SerializeField] private Camera TPC;
	[SerializeField] float minDistance;
	[SerializeField] float maxDistance;

	[Header("Overlays")]
	[SerializeField] private PlayerShoot playerShoot;
	[SerializeField] private GameObject defaultCrosshair;
	[SerializeField] private GameObject sniperOverlay;

	[System.Serializable]
	public class CameraRig
    {
		public Vector3 cameraOffset;
		public float crouchHeight;
	}

	[SerializeField] private CameraRig defaultCamera;
	[SerializeField] private CameraRig aimCamera;
	[SerializeField] private CameraRig sniperRig;
	private CameraRig cameraRig;
	private Player localPlayer; //The playerGameObject having player script on it
	private float targetHeight;

    private void Awake()
    {
		GameManager.Instance.OnLocalPlayerJoined += HandleOnLocalPlayerJoined;
    }

	void HandleOnLocalPlayerJoined(Player player)
    {
		localPlayer = player;
    }


	private void Start()
    {
		TPC = gameObject.GetComponent<Camera>();
		origionalMouseSenstivity = mouseSenstivity;

		defaultCrosshair.SetActive(true);
		sniperOverlay.SetActive(false);
		playerShoot.ActiveWeapon.projectile.damage = 1;
	}

    private void LateUpdate()
    {
		
		//Camera States
		cameraRig = defaultCamera;
		
		if(localPlayer.PlayerHealth.IsAlive) //check if player is alive
        {
			CheckAim();
		}
        else
        {
			ResetOnDeath();
        }

		CameraRotate();		      
		CameraCollisionDetection();
	}

    private void CheckAim()
    {
		if (localPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.AIMING || localPlayer.PlayerState.WeaponState == PlayerState.EWeaponState.AIMEDFIRING) 
		{
			cameraRig = aimCamera;
			CheckForScopedWeapon();

		}
		else
		{
			cameraRig = defaultCamera;
			defaultCrosshair.SetActive(true);
			sniperOverlay.SetActive(false);
			TPC.fieldOfView = 60;
			playerShoot.ActiveWeapon.projectile.damage = 1;
			mouseSenstivity = origionalMouseSenstivity;
		}
	}

	private void ResetOnDeath()
    {
		cameraRig = defaultCamera;
		defaultCrosshair.SetActive(false);
		sniperOverlay.SetActive(false);
		TPC.fieldOfView = 60;
		playerShoot.ActiveWeapon.projectile.damage = 1;
		mouseSenstivity = origionalMouseSenstivity;
	}

    private void CheckForScopedWeapon()
    {		
		if (playerShoot.ActiveWeapon.reloader.weaponType == EWeaponTypes.SNIPER)
		{
			if(localPlayer.PlayerHealth.IsAlive == false)
            {
				return;
            }
			//Check if we have sniper equipped
			cameraRig = sniperRig;
			playerShoot.ActiveWeapon.projectile.damage = 10;
			mouseSenstivity = 1f;
			defaultCrosshair.SetActive(false);
			sniperOverlay.SetActive(true);
			TPC.fieldOfView = 6;
		}
       
	}

    private void CameraRotate()
    {
		yaw += Input.GetAxis("Mouse X") * mouseSenstivity;
		pitch -= Input.GetAxis("Mouse Y") * mouseSenstivity;

		pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

		currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);

		transform.eulerAngles = currentRotation;

		targetHeight = cameraRig.cameraOffset.y + (localPlayer.PlayerState.MoveState == PlayerState.EMoveState.CROUCHING ? cameraRig.crouchHeight : 0 );

		transform.position = Vector3.Lerp(transform.position, target.position - (transform.forward * cameraRig.cameraOffset.z + transform.right * cameraRig.cameraOffset.x + transform.up * targetHeight),  5);
	}

	private void CameraCollisionDetection()
    {
		//For Camera Collision
		//Debug.DrawLine( target.position, transform.position, Color.red);
		RaycastHit hit;
		if (Physics.Linecast(target.position, transform.position, out hit))
		{
			//stop camera motion in backward direction 
			if (!hit.collider.CompareTag("MainCamera"))
			{
				transform.position = Vector3.Lerp(transform.position, new Vector3(hit.point.x, hit.point.y, hit.point.z), 2);
			}
			else if (hit.collider.CompareTag("MainCamera"))
			{
				//throw back to origional
				transform.position = Vector3.Lerp(transform.position, target.position - (transform.forward * cameraRig.cameraOffset.z + transform.right * cameraRig.cameraOffset.x + transform.up * targetHeight), Time.deltaTime * 5);
			}
		}
	}



}
