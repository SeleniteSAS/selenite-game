using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorBehavior : MonoBehaviour
{
    public Transform player;
    public Transform floorPosition;
    public Transform bridgePosition;
    public Animator doorAnim;
    public Transform cameraTransform;
    // Start is called before the first frame update

    void Teleport(){
            StartCoroutine(TeleportAnimation());
    }

    IEnumerator TeleportAnimation()
    {
        if (doorAnim.GetBool("isOpen")){
            doorAnim.SetBool("isOpen",false);
        }else{
            doorAnim.SetBool("isOpen",true);
        }

        yield return new WaitForSeconds(1f);

        Vector3 cameraOffset = cameraTransform.localPosition;

        if(player.position.y < bridgePosition.position.y){
            
            player.position = bridgePosition.position - new Vector3(cameraOffset.x , 0 , cameraOffset.z);
            
        }else{
            
            player.position = floorPosition.position - new Vector3(cameraOffset.x , 0 , cameraOffset.z);
            
        }

        yield return new WaitForSeconds(1f);

        if (doorAnim.GetBool("isOpen")){
            doorAnim.SetBool("isOpen",false);
        }else{
            doorAnim.SetBool("isOpen",true);
        }


    }

}
