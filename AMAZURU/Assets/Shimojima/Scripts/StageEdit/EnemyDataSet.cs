using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class EnemyDataSet : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyMaster;

    [SerializeField]
    private GameObject enemyContent, enemyDataItem, movePlanItem;

    private int enemyCount = 0;
    public int selectDataNum;

    public struct PositionData
    {
        public float x;
        public float y;
        public float z;
    }

    public struct EditEnemyData
    {
        public GameObject movePlanContent;
        public Text number;
        public Dropdown _enemyType, rotateDirection, moveType;
        public InputField enemySize, startPosX, startPosY, startPosZ, _normalSpeed, _waterSpeed, plusHight;
        public Toggle hasStartPos, setDefaultSpeed, isViewMovePlan;

        public List<PositionData> pData;

        public void Init(GameObject eDataItem)
        {
            SetDataItem sdi = eDataItem.GetComponent<SetDataItem>();
            movePlanContent = sdi.content;
            _enemyType = sdi.enemyType;
            rotateDirection = sdi.rotateDirection;
            moveType = sdi.moveType;
            enemySize = sdi.enemySize;
            startPosX = sdi.startPosX;
            startPosY = sdi.startPosY;
            startPosZ = sdi.startPosZ;
            _normalSpeed = sdi.normalSpeed;
            _waterSpeed = sdi.waterSpeed;
            plusHight = sdi.plusHight;
            hasStartPos = sdi.hasStartPos;
            setDefaultSpeed = sdi.setDefaultSpeed;
            isViewMovePlan = sdi.isViewMovePlan;
            number = sdi.number;
            pData = new List<PositionData>();
        }

        public void Save(GameObject enemyMaster)
        {
            EnemyMaster e = enemyMaster.GetComponent<EnemyMaster>();
            //movePlanContent = sdi.content;
            //_enemyType = sdi.enemyType;
            //rotateDirection = sdi.rotateDirection;
            //moveType = sdi.moveType;
            //enemySize = sdi.enemySize;
            //startPosX = sdi.startPosX;
            //startPosY = sdi.startPosY;
            //startPosZ = sdi.startPosZ;
            //_normalSpeed = sdi.normalSpeed;
            //_waterSpeed = sdi.waterSpeed;
            //plusHight = sdi.plusHight;
            //hasStartPos = sdi.hasStartPos;
            //setDefaultSpeed = sdi.setDefaultSpeed;
            //isViewMovePlan = sdi.isViewMovePlan;
            //number = sdi.number;
        }
    }

    public List<EditEnemyData> eed = new List<EditEnemyData>();

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveEnemyMaster();
            }
        }
    }

    public void AddData()
    {
        GameObject o = Instantiate(enemyDataItem);
        o.SetActive(true);
        o.transform.SetParent(enemyContent.transform, false);
        o.name = o.name + enemyCount.ToString();
        EditEnemyData eEnemyData = new EditEnemyData();
        eEnemyData.Init(o);
        eEnemyData.number.text = enemyCount.ToString();
        eed.Add(eEnemyData);
        enemyCount++;
    }

    public void AddMovePlan()
    {
        GameObject o = Instantiate(movePlanItem);
        o.transform.SetParent(eed[selectDataNum].movePlanContent.transform, false);
        o.transform.GetChild(0).name = o.transform.GetChild(0).name + eed[selectDataNum].pData.Count;
        o.transform.GetChild(1).name = o.transform.GetChild(1).name + eed[selectDataNum].pData.Count;
        o.transform.GetChild(2).name = o.transform.GetChild(2).name + eed[selectDataNum].pData.Count;
        PositionData p = new PositionData();
        eed[selectDataNum].pData.Add(p);
    }

    private void SaveEnemyMaster() 
    {
        for (int i = 0; i < enemyCount; i++)
        {
            EnemyData _enemyData = new EnemyData();
            _enemyData.Type = (EnemyType)eed[i]._enemyType.value;
            _enemyData.StartRotate = (EnemyData.RotateDirection)eed[i].rotateDirection.value;
            _enemyData.MoveType = (EnemyMoveType)eed[i].moveType.value;
            _enemyData.Size = float.Parse(eed[i].enemySize.text);
            _enemyData.UseStartPosSetting = eed[i].hasStartPos.isOn;
            if (eed[i].hasStartPos.isOn) 
            {
                _enemyData.StartPosition = new Vector3(float.Parse(eed[i].startPosX.text), float.Parse(eed[i].startPosY.text), float.Parse(eed[i].startPosZ.text));
            }
            _enemyData.UseDefaultSetting = eed[i].setDefaultSpeed.isOn;
            if (!eed[i].setDefaultSpeed.isOn)
            {
                _enemyData.NomalSpeed = float.Parse(eed[i]._normalSpeed.text);
                _enemyData.WaterSpeed = float.Parse(eed[i]._waterSpeed.text);
            }
            if (_enemyData.Type == EnemyType.Dry)
            {
                _enemyData.BlockSetPosY = int.Parse(eed[i].plusHight.text);
            }
            _enemyData.MovePlan = new Vector3[eed[i].pData.Count];
            for (int j = 0; j < _enemyData.MovePlan.Length; j++)
            {
                float x = eed[i].pData[j].x;
                float y = eed[i].pData[j].y;
                float z = eed[i].pData[j].z;
                _enemyData.MovePlan[j] = new Vector3(x, y, z);
            }
            AssetDatabase.CreateAsset(_enemyData, "Assets/Shimojima/shimojima_EnemyData" + i + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
}
