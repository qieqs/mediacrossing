using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class CharacterManager : MonoBehaviour
{
    public string lopen_voorwaards_zin;
    public string lopen_zijwaards_zin;
    public string lopen_achterwaards_zin;
    public string actie_zin;
    private string currentaction;

    public float loopsnelheid;
    private Animator animator;
    private SpriteRenderer spriterender;

    private IEnumerator routine;
    private NavMeshAgent agent;
    public NavMeshPath navMeshPath;

    public float idleprob = 0.25f;
    public float actionprob = 0.25f;
    public float prob = 0.5f;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        spriterender = GetComponentInChildren<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();
        ChooseAction();
    }

    void Update()
    {
        
    }

    public void ChooseAction()
    {
        if(routine != null)
        {
            StopCoroutine(routine);
        }
        int action = actionChanceCalculation();
        Debug.Log(action);
        switch (action)
        {
            case 0: //start loop actie
                FindTarget();
                break;
            case 1: //start idle
                startidle();
                break;
            case 2: //start action
                performAction();
                break;
            default:
                break;
        }
    }

    public int actionChanceCalculation()
    {
        float randomaction = Random.value;
        if(randomaction < idleprob)
        {
            idleprob -= 0.05f;
            return 1;
        }
        else if(randomaction < actionprob)
        {
            actionprob -= 0.05f;
            return 2;
        }
        else
        {
            idleprob += 0.05f;
            actionprob += 0.05f;
            return 0;
        }
    }

    void performAction()
    {
        routine = performaction();
        StartCoroutine(routine);
    }

    private IEnumerator performaction()
    {
        if(actie_zin == "")
        {
            ChooseAction();
        }
        else
        {
            currentaction = actie_zin;
            animator.SetBool(currentaction, true);
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }
            animator.SetBool(currentaction, false);
            ChooseAction();
        }
    }

    void startidle()
    {
        routine = idle();
        StartCoroutine(routine);
    }

    private IEnumerator idle()
    {
        yield return new WaitForSeconds(Random.Range(5,10));
        ChooseAction();
    }


    void FindTarget()
    {
        navMeshPath = new NavMeshPath();
        Vector3 targetposition = ChanceCalculation();
        if(CalculateNewPath(targetposition) == true)
        {
            routine = WalkToTarget(targetposition);
            StartCoroutine(routine);
        }
        else
        {
            FindTarget();
        }
    }

    Vector3 ChanceCalculation()
    {
        float randomdirection = Random.Range(-10f,10f);
        if (Random.value > prob)
        {
            prob = prob + 0.2f;
            if(randomdirection < 0)
            {
                currentaction = lopen_zijwaards_zin;
            }
            else
            {
                currentaction = lopen_zijwaards_zin;
            }
            return new Vector3(randomdirection, transform.position.y, transform.position.z);
        }
        else
        {
            prob = prob - 0.2f;
            if (randomdirection < 0)
            {
                currentaction = lopen_voorwaards_zin;
            }
            else
            {
                currentaction = lopen_achterwaards_zin;
            }
            return new Vector3(transform.position.x, transform.position.y, randomdirection);
        }
    }

    bool CalculateNewPath(Vector3 targetPosition)
    {
        agent.CalculatePath(targetPosition, navMeshPath);
        if (navMeshPath.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private IEnumerator WalkToTarget(Vector3 target)
    {
        animator.SetBool(currentaction, true);
        while(Vector3.Distance(target, transform.position) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, loopsnelheid * Time.deltaTime);
            yield return null;
        }
        animator.SetBool(currentaction, false);
        ChooseAction();
    }
}

