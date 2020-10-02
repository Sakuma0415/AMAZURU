using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shimojima{
    namespace StageEditUtility
    {
        public class StageEditUtility : MonoBehaviour
        {
            /// <summary>
            /// stageObjAngleに引数で設定された数値を足す
            /// </summary>
            /// <param name="v"></param>
            private static Vector3 AdditionStageObjAngle(Vector3 angle, Vector3 v = default)
            {
                if (v == Vector3.zero) { return angle; }
                angle += v;

                int index = CheckAngleOverflowForStageObject(angle);

                switch (index)
                {
                    case 0:
                        angle.x = 0;
                        break;
                    case 1:
                        angle.y = 0;
                        break;
                    case 2:
                        angle = Vector3.zero;
                        break;
                    default:
                        break;
                }

                return angle;
            }

            /// <summary>
            /// X,Yまたはその両方が絶対値で360を超えているか否かの判断
            /// </summary>
            /// <param name="v">stageObjAngle</param>
            /// <returns>index</returns>
            private static int CheckAngleOverflowForStageObject(Vector3 v)
            {
                int index = -1;

                //z値の値は変動しない為省略
                bool isOverflowX = false;
                bool isOverflowY = false;

                if (Mathf.Abs(v.x) == 360) { index = 0; isOverflowX = true; }
                else if (Mathf.Abs(v.y) == 360) { index = 1; isOverflowY = true; }

                if (isOverflowX && isOverflowY) { index = 2; }

                return index;
            }

            /// <summary>
            /// オブジェクトをX軸で90度回転させる
            /// </summary>
            /// <param name="target"></param>
            /// <param name="guideObj"></param>
            /// <param name="angle"></param>
            /// <returns>移動処理後のguideObj, 加算処理後のangle</returns>
            public static (GameObject, Vector3) RotationX(GameObject target, GameObject guideObj, Vector3 angle)
            {
                Camera.main.transform.SetParent(null);
                Vector3 a = AdditionStageObjAngle(angle, new Vector3(90, 0, 0)); ;
                guideObj.transform.localEulerAngles = a;
                target.transform.SetParent(null);
                Camera.main.transform.SetParent(guideObj.transform);
                target.transform.SetParent(guideObj.transform);
                return (guideObj, a);
            }

            /// <summary>
            /// オブジェクトをY軸で-90度回転させる
            /// </summary>
            /// <param name="target"></param>
            /// <param name="guideObj"></param>
            /// <param name="angle"></param>
            /// <returns>移動処理後のguideObj, 加算処理後のangle</returns>
            public static (GameObject, Vector3) RotationY(GameObject target, GameObject guideObj, Vector3 angle)
            {
                Camera.main.transform.SetParent(null);
                Vector3 a = AdditionStageObjAngle(angle, new Vector3(0, -90, 0)); ;
                guideObj.transform.localEulerAngles = a;
                target.transform.SetParent(null);
                Camera.main.transform.SetParent(guideObj.transform);
                target.transform.SetParent(guideObj.transform);
                return (guideObj, a);
            }

            /// <summary>
            /// 上入力キーが押されたか
            /// <para>W, UpArrow</para>
            /// </summary>
            /// <returns></returns>
            public static bool GetUpKeyDown()
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// 下入力キーが押されたか
            /// <para>S, DownArrow</para>
            /// </summary>
            /// <returns></returns>
            public static bool GetDownKeyDown()
            {
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// 右入力キーが押されたか
            /// <para>D, RightArrow</para>
            /// </summary>
            /// <returns></returns>
            public static bool GetRightKeyDown()
            {
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// 左入力キーが押されたか
            /// <para>A, LeftArrow</para>
            /// </summary>
            /// <returns></returns>
            public static bool GetLeftKeyDown()
            {
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    return true;
                }

                return false;
            }
        }
    }
}

