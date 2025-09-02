using System;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    private StarterAssetsInputs input;
    private ThirdPersonController controller;
    private Animator anim;
    
    [Header("Aim")] 
    [SerializeField] private CinemachineVirtualCamera aimCam;
    [SerializeField] private GameObject aimImage;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject aimObj;
    [SerializeField] private float aimObjDis;

    [Header("IK")] 
    [SerializeField]private Rig handRig;
    [SerializeField]private Rig aimRig;
    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<ThirdPersonController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        AimCheck();
    }

    private void AimCheck()
    {
        if (input.reload)
        {
            input.reload = false;

            if (controller.isReLoad)
            {
                return;
            }

            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1,1);
            anim.SetTrigger("Reload");
            controller.isReLoad = true;
        }

        if (controller.isReLoad)
        {
            return;
        }

        if (input.aim)
        {
            AimControll(true);
            anim.SetLayerWeight(1, 1);
            Vector3 targetPosition = Vector3.zero;
            Transform camTransform = Camera.main.transform;
            RaycastHit hit;
            
            if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, Mathf.Infinity,targetLayer))
            {
                Debug.Log("Name :  "+ hit.transform.gameObject.name);
                targetPosition = hit.point;
                aimObj.transform.position = hit.point;
            }
            else
            {
                targetPosition = camTransform.position +  camTransform.forward *aimObjDis ;
                aimObj.transform.position = camTransform.position +  camTransform.forward*aimObjDis;
            }

            Vector3 targetAim = targetPosition;
            targetAim.y = transform.position.y;
            Vector3 aimDir = (targetAim - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime*50);
            SetRigWeight(1);
            if (input.shoot)
            {
                anim.SetBool("Shoot", true);
                GameManager.instance.Shooting(targetPosition);
            }
            else
            {
                anim.SetBool("Shoot", false);
            }

        }
        else//  조준 x 
        {
            AimControll(false);
            SetRigWeight(0);
            anim.SetLayerWeight(1, 0);
            anim.SetBool("Shoot", false);
        }
    }

    private void AimControll(bool isCheck)
    {
        aimCam.gameObject.SetActive(isCheck);
        aimImage.SetActive(isCheck);
        controller.isAimMove = isCheck;
    }

    public void Reroad()
    {
        //Debug.Log("Reload");
        controller.isReLoad= false;
        SetRigWeight(1);
        anim.SetLayerWeight(1,0);
    }

    public void SetRigWeight(float weight)
    {
        aimRig.weight = weight;
        handRig.weight = weight; 
    }

    public void ReLoadWeaponClip()
    {
        GameManager.instance.ReLoadClip();
    }
}
