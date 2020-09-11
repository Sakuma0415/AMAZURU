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
    private GameObject e;

    public string sName;
    public GameObject stageRoot;

    [SerializeField]
    private GameObject enemyContent, enemyDataItem, movePlanItem;

    private int enemyCount = 0;
    public int selectDataNum;

    public bool createEnemy = false;

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
        public Dropdown _enemyType, rotateDirection, moveType, enemyVecotrUp;
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
            enemyVecotrUp = sdi.enemyVectorUp;
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

        public void DataSet(EnemyData eData, int num)
        {
            _enemyType.value = (int)eData.Type;
            rotateDirection.value = (int)eData.StartRotate;
            moveType.value = (int)eData.MoveType;
            enemyVecotrUp.value = (int)eData.EnemyUpDirection;
            enemySize.text = eData.Size.ToString();
            hasStartPos.isOn = eData.UseStartPosSetting;
            if (eData.UseStartPosSetting)
            {
                startPosX.text = eData.StartPosition.x.ToString();
                startPosY.text = eData.StartPosition.y.ToString();
                startPosZ.text = eData.StartPosition.z.ToString();
            }
            setDefaultSpeed.isOn = eData.UseDefaultSetting;
            if (!eData.UseDefaultSetting)
            {
                _normalSpeed.text = eData.NomalSpeed.ToString();
                _waterSpeed.text = eData.WaterSpeed.ToString();
            }
            if (eData.Type == EnemyType.Dry)
            {
                plusHight.interactable = true;
                plusHight.text = eData.BlockSetPosY.ToString();
            }
        }
    }

    public List<EditEnemyData> eed = new List<EditEnemyData>();

    public void AddData()
    {
        if (!createEnemy)
        {
            e = Instantiate(enemyMaster);
            e.transform.SetParent(stageRoot.transform);
            e.name = enemyMaster.name;
            createEnemy = true;
        }
        GameObject o = Instantiate(enemyDataItem);
        o.name = enemyDataItem.name + ":" + enemyCount.ToString();
        o.SetActive(true);
        o.transform.SetParent(enemyContent.transform, false);
        EditEnemyData eEnemyData = new EditEnemyData();
        eEnemyData.Init(o);
        eEnemyData.number.text = enemyCount.ToString();
        eed.Add(eEnemyData);
        enemyCount++;
    }

    public void AddMovePlan(int i = -1)
    {
        if (i == -1)
        {
            i = selectDataNum;
        }
        GameObject o = Instantiate(movePlanItem);
        o.name = movePlanItem.name + ":" + eed[i].pData.Count;
        o.transform.SetParent(eed[i].movePlanContent.transform, false);
        o.transform.GetChild(0).GetComponent<Text>().text = eed[i].pData.Count.ToString();
        o.transform.GetChild(1).name = o.transform.GetChild(1).name + eed[i].pData.Count;
        o.transform.GetChild(2).name = o.transform.GetChild(2).name + eed[i].pData.Count;
        o.transform.GetChild(3).name = o.transform.GetChild(3).name + eed[i].pData.Count;
        PositionData p = new PositionData();
        eed[i].pData.Add(p);
    }

    public void SaveEnemyMaster() 
    {
        e.GetComponent<EnemyMaster>().EnemyDataArray = new EnemyData[enemyCount];

        if(!System.IO.Directory.Exists(Application.dataPath + "/Hara/Data/EnemyData/" + sName))
        {
            AssetDatabase.CreateFolder("Assets/Hara/Data/EnemyData", sName);
        }

        for (int i = 0; i < enemyCount; i++)
        {
            EnemyData _enemyData = new EnemyData();
            _enemyData.Type = (EnemyType)eed[i]._enemyType.value;
            _enemyData.StartRotate = (EnemyData.RotateDirection)eed[i].rotateDirection.value;
            _enemyData.MoveType = (EnemyMoveType)eed[i].moveType.value;
            _enemyData.EnemyUpDirection = (EnemyData.UpDirection)eed[i].enemyVecotrUp.value;
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
            AssetDatabase.CreateAsset(_enemyData, "Assets/Hara/Data/EnemyData/"+ sName+ "/" + sName + "-" + i + ".asset");
            AssetDatabase.SaveAssets();
            e.GetComponent<EnemyMaster>().EnemyDataArray[i] = _enemyData;
        }
    }

    public void LoadEnemyData(GameObject EMaster)
    {
        e = EMaster;
        EnemyMaster eMaster = EMaster.GetComponent<EnemyMaster>();
        for (int i = 0; i < eMaster.EnemyDataArray.Length; i ++)
        {
            AddData();
            eed[i].DataSet(eMaster.EnemyDataArray[i], i);
            Vector3[] v = eMaster.EnemyDataArray[i].MovePlan;
            for (int j = 0; j < v.Length; j++)
            {
                AddMovePlan(i);
                PositionData p = new PositionData();
                p.x = v[j].x;
                p.y = v[j].y;
                p.z = v[j].z;

                GameObject mp = eed[i].movePlanContent.transform.GetChild(j).gameObject;
                mp.transform.GetChild(1).GetComponent<InputField>().text = p.x.ToString();
                mp.transform.GetChild(2).GetComponent<InputField>().text = p.y.ToString();
                mp.transform.GetChild(3).GetComponent<InputField>().text = p.z.ToString();

                eed[i].pData[j] = p;

                
            }
        }
    }
}
