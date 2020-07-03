using UnityEngine;
using Leap;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
/// <summary>
/// 当检测到一只手后，进入追踪状态，记录单手运动的速度，掌心方向。
/// 检测到HandOpen之后，延迟0.5s进入追踪状态。
/// </summary>
public class SingleHandOpenTrack : MonoBehaviour {
    [SerializeField] HandOpen m_HandOpen;
    Controller leap;
    int dirZ, dirX, dirY,dirS;

     
    //Update设定
    readonly float TrackStepTime = 0.1f;
    float m_CurTrackTime;

    //初始状态
    [SerializeField]
    bool m_IsSingleHandOpened;
    readonly float EnterDelayTime = 0.50f;//进入0.5s后触发
    float m_CurEnterDelayTime;
    Action m_OnEnterFunc;

    //运动状态
	[SerializeField] float m_Speed;//手掌运动速度，前后两个位置做差
    Vector m_PreviousPos;//前次的位置
    Vector m_Dir=Vector.Zero;//手掌运动的方向，CurPos-PrePos

    //结束状态
    Action m_OnEndFunc;
    public bool IsSingleHandOpened
    {
        get
        {
            return m_IsSingleHandOpened;
        }
    }
    
    public Vector Dir
    {
        get
        {
            return m_Dir;
        }
    }
    public string DirS
    {
        get
        {
            string dir;
            if (dirS == 0) dir = "None";
            else if (dirS == 1) dir = "Still";
            else dir = "Other";
            return dir;
        }
    }
    public string DirZ
    {
        get
        {
            string dir;
            if (dirZ == 0) dir = "None";
            else if (dirZ == 1) dir = "Forward";
            else dir = "Backward";
            return dir;
        }
    }

    public string DirX
    {
        get
        {
            string dir;
            if (dirX == 0) dir = "None";
            else if (dirX == 1) dir = "Right";
            else dir = "Left";
            return dir;
        }
    }
    public string DirY
    {
        get
        {
            string dir;
            if (dirY == 0) dir = "None";
            else if (dirY == 1) dir = "Up";
            else dir = "Down";
            return dir;
        }
    }

    public float Speed
    {
        get
        {
            return m_Speed;
        }
    }

    public void RegisterFunc(Action m_OnEnter,Action m_OnEnd)
    {
        if(m_OnEnter!=null)
        {
            m_OnEndFunc += m_OnEnter;
        }
        if(m_OnEnd!=null)
        {
            m_OnEndFunc += m_OnEnd;
        }
    }

    public void CancelFunc(Action m_OnEnter, Action m_OnEnd)
    {
        if (m_OnEnter != null)
        {
            m_OnEndFunc -= m_OnEnter;
        }
        if (m_OnEnd != null)
        {
            m_OnEndFunc -= m_OnEnd;
        }
    }
    void Awake()
    {
        leap = LeapDriver.GetLeapCtrl();
        dirZ = dirX = 0;
    }
	// Update is called once per frame
	void Update () 
    {

        m_CurTrackTime +=Time.deltaTime;
        m_CurEnterDelayTime += Time.deltaTime;
        
        if(m_CurTrackTime>TrackStepTime)
        {
            m_CurTrackTime = 0f;
            //print(leap.Frame().Hands.Count);
            if (leap.Frame().Hands.Count == 1)
            {
                int index;
                int number = m_HandOpen.OpenNumber(out index);
                //只有一只手被检测到
            
                //如果只有一只手处于摊开状态
                if (number == 1)
                {
                    //第一次伸掌
                    if (!m_IsSingleHandOpened)
                    {
                        //满足延迟时间
                        if (m_CurEnterDelayTime > EnterDelayTime)
                        {
                            m_CurEnterDelayTime = 0f;

                            m_IsSingleHandOpened = true;
                            m_PreviousPos = m_HandOpen.PalmPos[index];
                            if (m_OnEndFunc != null)
                            {
                                m_OnEndFunc();
                            }
                        }
                    }
                    //已经处于伸掌状态
                    else
                    {
                        Vector curDir = m_HandOpen.PalmPos[index];
                        m_Dir = curDir - m_PreviousPos;    
                        m_Speed = curDir.DistanceTo(m_PreviousPos);
                        //判断手掌运动方向，X轴正方向为右，Z轴正方向为下, Y轴正方向为上;同时判断手掌是否长期处于某一手势状态
                       
                        //Control by gesture
                        if (m_Dir.x > 20 && curDir.x > 30 && m_Dir.z < 10 && m_Dir.z > -10)
                        {
                            dirX = 1;
                            dirZ = 0;
                            dirY = 0;
                            dirS = 0;
                        }
                        else if (m_Dir.x < -20 && curDir.x < -30 && m_Dir.z < 10 && m_Dir.z > -10)
                        {
                            dirX = -1;
                            dirZ = 0;
                            dirY = 0;
                            dirS = 0;
                        }

                        //else if (curDir.x > 40 && curDir.DistanceTo(m_PreviousPos) < 10)
                        //{
                        //    dirX = 1;
                        //    dirZ = 0;
                        //    dirY = 0;
                        //}
                        //else if (curDir.x < -40 && curDir.DistanceTo(m_PreviousPos) < 10)
                        //{
                        //    dirX = -1;
                        //    dirZ = 0;
                        //    dirY = 0;
                        //}

                        if (m_Dir.z > 20 && curDir.z > 30 && m_Dir.x < 10 && m_Dir.x > -10)
                        {
                            dirZ = -1;
                            dirX = 0;
                            dirY = 0;
                            dirS = 0;
                        }
                        else if (m_Dir.z < -20 && curDir.z < -30 && m_Dir.x < 10 && m_Dir.x > -10)
                        {
                            dirZ = 1;
                            dirX = 0;
                            dirY = 0;
                            dirS = 0;
                        }
                        //else if (curDir.z > 40 && curDir.DistanceTo(m_PreviousPos) < 10)
                        //{
                        //    dirZ = -1;
                        //    dirX = 0;
                        //    dirY = 0;
                        //}
                        //else if (curDir.z < -40 && curDir.DistanceTo(m_PreviousPos) < 10)
                        //{
                        //    dirZ = 1;
                        //    dirX = 0;
                        //    dirY = 0;
                        //}
                        else if (m_Dir.y > 20  && m_Dir.x < 10 && m_Dir.x > -10 && m_Dir.z < 10 && m_Dir.z > -10)
                        {
                            dirY = 1;
                            dirX = 0;
                            dirZ = 0;
                            dirS = 0;
                        }
                        else if (m_Dir.y < -20  && m_Dir.x < 10 && m_Dir.x > -10 && m_Dir.z < 10 && m_Dir.z > -10)
                        {
                            dirY = -1;
                            dirX = 0;
                            dirZ = 0;
                            dirS = 0;
                        }
                        //else if (curDir.y > 100 && m_Dir.y < 10 && m_Dir.y > -10 && m_Dir.x < 10 && m_Dir.x > -10 && m_Dir.z < 10 && m_Dir.z > -10)
                        //{
                        //    dirY = 0;
                        //    dirX = 0;
                        //    dirZ = 0;
                        //    dirS = 1;
                        //}
                        m_PreviousPos = curDir;


                        //Control by position
                        //if (curDir.x > 50 && curDir.z < 20 && curDir.z > -20)
                        //{
                        //    dirX = 1;
                        //    dirY = 0;
                        //    dirZ = 0;
                        //}
                        //else if(curDir.x <- 50 && curDir.z < 20 && curDir.z > -20)
                        //{
                        //    dirX = -1;
                        //    dirY = 0;
                        //    dirZ = 0;
                        //}
                        //else if (curDir.z < -50 && curDir.x < 20 && curDir.x > -20)
                        //{
                        //    dirX = 0;
                        //    dirY = 0;
                        //    dirZ = 1;
                        //}
                        //else if (curDir.z > 50 && curDir.x < 20 && curDir.x > -20)
                        //{
                        //    dirX = 0;
                        //    dirY = 0;
                        //    dirZ = -1;
                        //}
                        //else if (m_Dir.y > 40 && m_Dir.x < 20 && m_Dir.x > -20 && m_Dir.z < 20 && m_Dir.z > -20)
                        //{
                        //    if (dirY==0)
                        //    {
                        //        dirY = 1;
                        //        dirX = 0;
                        //        dirZ = 0;
                        //    }
                        //}
                    }
                }
                //不是伸掌状态
                else
                {
                    //上一次是伸掌状态，说明这是一次手势的结束，触发结束事件
                    if (m_IsSingleHandOpened)
                    {
                        if (m_OnEndFunc != null)
                        {
                            m_OnEndFunc();
                        }
                    }
                    //初始化数据
                    Reset();
                }
            }
            //不是伸掌状态
            else
            {
                //上一次是伸掌状态，说明这是一次手势的结束，触发结束事件
                if (m_IsSingleHandOpened)
                {
                    if (m_OnEndFunc != null)
                    {
                        m_OnEndFunc();
                    }
                }
                //初始化数据
                Reset();
            }
        }
       
	}
    /// <summary>
    /// 重置数据，如果判定失败时调用
    /// </summary>
    void Reset()
    {
        m_Speed = 0f;
        m_PreviousPos = Vector.Zero;
        m_Dir = Vector.Zero;
        m_IsSingleHandOpened = false;
//        m_CurTrackTime = 0f;
        m_CurEnterDelayTime = 0f;
        dirX = dirZ = dirY = dirS = 0;
    }
}
