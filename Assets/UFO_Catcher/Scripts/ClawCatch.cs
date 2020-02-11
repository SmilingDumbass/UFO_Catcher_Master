using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawCatch : MonoBehaviour
{
    public float maxTime;
    public float downSpeed;
    public float catchSpeed;
    public float returnSpeed;

    public float catchTime;
    public float relaxTime; // relaxTime < catchTime
    public float dropProbability;

    private Transform claw;
    private Transform circle;
    private string[] stage = {
        "idle",
        "moving", // when entering this stage, give permission to player to control the claw
        "landing", // when entering this stage, reclaim permission from player
        "catching",
        "pulling",
        "returning",
        "droping"
    };
    private int stageIndex = 0;
    private float timer = 0;
    private float downAccumulation = 0;
    private float catchAccumulation = 0;
    private Vector3 idlePosition;
    private Vector3 circleLocalIdle;
    private Vector3 returnDirection;
    private float relaxThreshold;
    private bool relax = false;
    private float catch_timer = 0;

    void Start()
    {
        claw = transform.Find("Claw");
        circle = transform.Find("Base").Find("Circle");
        timer = maxTime;
        idlePosition = claw.position;
        circleLocalIdle = circle.localPosition;
    }

    void Update()
    {
        if (stage[stageIndex] == "idle")
        {
            claw.position = idlePosition;
            // EnterCoins() handles stageIndex++
        }
        else if (stage[stageIndex] == "moving")
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                Stretch();
            // Stretch() handles stageIndex++
        }
        else if (stage[stageIndex] == "landing")
        {
            float translation = downSpeed * Time.deltaTime;
            claw.Translate(new Vector3(0, -translation, 0), Space.World);
            downAccumulation += -translation;
            // OnDetectorHit() handles stageIndex++
        }
        else if (stage[stageIndex]=="catching")
        {
            float translation = catchSpeed * Time.deltaTime;
            circle.Translate(new Vector3(0, 0, translation), Space.Self);
            catchAccumulation += translation;
            catch_timer += Time.deltaTime;
            if (catch_timer > catchTime)
            {
                catch_timer = 0;
                stageIndex++;
            }
        }
        else if (stage[stageIndex] == "pulling")
        {
            float translation = downSpeed * Time.deltaTime;
            if (downAccumulation + translation >= 0)
            {
                translation = -downAccumulation;
                stageIndex++;
                returnDirection = idlePosition - claw.position;
                returnDirection.y = 0;
                relaxThreshold = GetThreshold(returnDirection.magnitude);
                returnDirection = returnDirection.normalized;
                relax = true;
            }
            claw.Translate(new Vector3(0, translation, 0), Space.World);
            downAccumulation += translation;
        }
        else if (stage[stageIndex] == "returning")
        {
            float translation = returnSpeed * Time.deltaTime;
            float dist = Vector3.Distance(claw.position, idlePosition);
            if (translation >= dist)
            {
                claw.position = idlePosition;
                stageIndex++;
                catch_timer = 0;
                relax = false;
            }
            else
            {
                claw.Translate(returnDirection * translation, Space.World);
                if (dist <= relaxThreshold)
                {
                    Relax();
                }
            }
        }
        else if (stage[stageIndex] == "droping")
        {
            float translation = catchSpeed * Time.deltaTime;
            if (catchAccumulation - translation <= 0)
            {
                circle.localPosition = circleLocalIdle;
                catchAccumulation = 0;
                stageIndex = 0;
            }
            else
            {
                circle.Translate(new Vector3(0, 0, -translation), Space.Self);
                catchAccumulation += -translation;
            }
        }
    }

    public bool EnterCoins()
    {
        if (stage[stageIndex] == "idle")
        {
            stageIndex++;
            transform.SendMessage("Unlock");
            return true;
        }
        return false;
    }

    void Stretch()
    {
        if (stage[stageIndex] == "moving")
        {
            transform.SendMessage("Lock");
            stageIndex++;
            timer = maxTime;
        }
    }

    void OnDetectorHit()
    {
        if (stage[stageIndex] == "landing")
        {
            stageIndex++;
        }
    }

    private void Relax()
    {
        if (!relax)
        {
            return;
        }
        float translation = catchSpeed * Time.deltaTime;
        circle.Translate(new Vector3(0, 0, -translation), Space.Self);
        catchAccumulation += -translation;
        catch_timer += Time.deltaTime;
        if (catch_timer > relaxTime)
        {
            catch_timer = 0;
            relax = false;
        }
    }

    private float GetThreshold(float dist)
    {
        if (Random.value > dropProbability)
        {
            return -1;
        }
        return -1;
    }
}
