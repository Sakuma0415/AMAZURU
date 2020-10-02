using UnityEngine;
using UnityEngine.UI;

public class SetDataItem: MonoBehaviour
{
    public Text number;
    public GameObject content;
    public Dropdown enemyType, rotateDirection, moveType, enemyVectorUp;
    public InputField enemySize, startPosX, startPosY, startPosZ, normalSpeed, waterSpeed, plusHight;
    public Toggle hasStartPos, setDefaultSpeed, isViewMovePlan;

    public void IsOnStartPosInteract()
    {
        startPosX.interactable = !startPosX.interactable;
        startPosY.interactable = !startPosY.interactable;
        startPosZ.interactable = !startPosZ.interactable;
    }

    public void IsOnDryEnemyTypeDataInteract()
    {
        if (enemyType.captionText.text == "Dry")
        {
            plusHight.interactable = !plusHight.interactable;
        }
        else
        {
            plusHight.interactable = false;
        }
        
    }

    public void IsOnSpeedDataInteract()
    {
        normalSpeed.interactable = !normalSpeed.interactable;
        waterSpeed.interactable = !waterSpeed.interactable;
    }
}
