using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelect : MonoBehaviour
{

    [SerializeField]
    GameObject cardObj;
    [SerializeField]
    GameObject cardParent;
    [SerializeField]
    Vector3 deckPivot;

    [SerializeField]
    StageCardData[] StageList;
    [SerializeField]
    float span;

    int serectP;
    int centerP;
    int goP;
    [SerializeField]
    RectTransform CursorRect;
    RectTransform deckRect;


    float animeTime=0;
    float cAnimeTime = 0;
    Vector3 nowPos;
    Vector3 nextPos;
    [SerializeField]
    float animeSp;

    GameObject[] deck;

    // Start is called before the first frame update
    void Start()
    {
        CursorRect.anchoredPosition = deckPivot;
        serectP = 1;
        centerP = 1;
        goP = 1;
        CardMake();
        deckRect = cardParent.GetComponent<RectTransform>();
    }


    void CardMake()
    {
        deck = new GameObject[StageList.Length];
        for (int i = 0; i < StageList.Length; i++)
        {
            GameObject bf= Instantiate(cardObj,cardParent.transform);
            bf.GetComponent<StageCard>().CardMake(StageList[i].sprite, StageList[i].stageName , StageList[i].rank);
            bf.GetComponent<RectTransform>().anchoredPosition=new Vector3 ((i-1)*span,0,0)+ deckPivot;
            deck[i] = bf;
        }
    }



    // Update is called once per frame
    void Update()
    {
        Debug.Log(goP);
        if (Input.GetKey(KeyCode.A)&&animeTime ==0 && cAnimeTime == 0 )
        {
            goP--;
            if (serectP == 0)
            {
                if ( centerP > 1)
                {
                    centerP--;
                    animeTime = 1 / animeSp;
                    nextPos = (Vector3)deckRect.anchoredPosition + new Vector3(span, 0, 0);
                    nowPos = (Vector3)deckRect.anchoredPosition;
                }
                else
                {
                    goP++;
                }

            }
            else
            {
                serectP--;
                cAnimeTime = 1 / animeSp;
                nextPos = (Vector3)CursorRect.anchoredPosition - new Vector3(span, 0, 0);
                nowPos = (Vector3)CursorRect.anchoredPosition;
            }
            
        }

        if (Input.GetKey(KeyCode.D) && animeTime== 0 && cAnimeTime == 0 )
        {
            goP++;
            if (serectP == 2)
            {
                if (centerP < StageList.Length - 2)
                {
                    centerP++;
                    animeTime = 1 / animeSp;
                    nextPos = (Vector3)deckRect.anchoredPosition - new Vector3(span, 0, 0);
                    nowPos = (Vector3)deckRect.anchoredPosition;
                }
                else
                {
                    goP--;

                }
                
            }
            else
            {
                serectP++;
                cAnimeTime = 1 / animeSp;
                nextPos = (Vector3)CursorRect.anchoredPosition + new Vector3(span, 0, 0);
                nowPos = (Vector3)CursorRect.anchoredPosition;
            }
        }

        if(animeTime > 0)
        {
            animeTime -= Time.deltaTime;
            if(animeTime < 0) { animeTime = 0; }

            deckRect.anchoredPosition = new Vector3(Mathf.Lerp(nowPos.x, nextPos.x, 1 - animeTime*animeSp), 0, 0) ;

        }


        if (cAnimeTime > 0)
        {
            cAnimeTime -= Time.deltaTime;
            if (cAnimeTime < 0) { cAnimeTime = 0; }

            CursorRect.anchoredPosition = new Vector3(Mathf.Lerp(nowPos.x, nextPos.x, 1 - cAnimeTime * animeSp), 0, 0) + deckPivot;

        }







        //



        if(Input.GetKeyDown (KeyCode.Space))
        {

        }
    }
}
