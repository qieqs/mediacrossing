using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(CharacterController))]

public class SpelerController : MonoBehaviour
{
    [Header("connected controllers")]
    private CharacterController controller;
    private Animator animator;
    private SpriteRenderer spriterender;
    private NavMeshAgent agent;
    public NavMeshPath navMeshPath;
    public float loopsnelheid = 6f;

    [Header("animatie triggers")]
    public string verticale_trigger;
    public string horizontale_trigger;
    public List<string> Overige_trigger = new List<string>();
    private string currentaction;

    [Header("input")]
    private float horizontal;
    private float vertical;
    private float Gravity = -9.81f;
    private float gravityMultiplier = 3.0f;
    private float velocity;
    private Vector3 target;

    private bool npcMode;
    public bool playerMode;

    //npc variabelen
    private IEnumerator routine;

    [Header("chance variables")]
    private float idleprob = 0.25f;
    private float actionprob = 0.25f;
    private float prob = 0.5f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if(controller == null)
        {
            Debug.LogError("er zit geen charactercontroller op dit karakter. het zal niet werken zonder");
        }
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("er zit geen animator op dit object. het zal niet werken zonder");
        }
        spriterender = GetComponentInChildren<SpriteRenderer>();
        if (spriterender == null)
        {
            Debug.LogError("er zit geen spriterenderer op dit object. het zal niet werken zonder");
        }
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("er zit geen navmeshagent op dit object. het zal niet werken zonder");
        }
        CheckVariables();
    }

    void CheckVariables()
    {
        if(verticale_trigger == "" || horizontale_trigger == "")
        {
            Debug.LogError("je hebt geen naam ingevuld voor de horizontale en/of vertical triggers. als je animaties hebt geïmporteerd zullen deze niet werken");
        }
    }

    void Update()
    {
        if (playerMode)
        {
            PlayerControl();
        }
        else
        {
            NpcControl();
        }
    }
    private void NpcControl()
    {
        if(npcMode != true)
        {
            StartCoroutine(ChooseAnimation());
            ChooseAction();
            npcMode = true;
        }
    }
    public void ChooseAction()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        int action = actionChanceCalculation();
        switch (action)
        {
            case 0: //start loop actie
                SearchForATarget();
                break;
            case 1: //start idle
                startidle();
                break;
            case 2: //start loop actie
                Chooserandomaction();
                break;
            default:
                break;
        }
    }
    public int actionChanceCalculation()
    {
        float randomaction = Random.value;
        if (randomaction < idleprob)
        {
            idleprob -= 0.05f;
            return 1;
        }
        else if (randomaction < actionprob)
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
    private IEnumerator ChooseAnimation()
    {
        while (!playerMode)
        {
            animator.SetFloat(horizontale_trigger, horizontal);
            animator.SetFloat(verticale_trigger, vertical);

            if(Vector2.Distance(new Vector2(target.x, target.z), new Vector2(transform.position.x, transform.position.z)) > 1f)
            {
                if (target.x - transform.position.x > 0)
                {
                    horizontal = 1;
                }
                else if (target.x - transform.position.x < 0)
                {
                    horizontal = -1;
                }
                if (target.z - transform.position.z > 0)
                {
                    vertical = 1;
                }
                else if (target.z - transform.position.z < 0)
                {
                    vertical = -1;
                }
            }
            else
            {
                horizontal = 0;
                vertical = 0;
            }
            yield return null;
        }
    }
    void startidle()
    {
        routine = idle();
        StartCoroutine(routine);
    }
    private IEnumerator idle()
    {
        target = transform.position;
        yield return new WaitForSeconds(Random.Range(3,5));
        ChooseAction();
    }
    private void SearchForATarget()
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 testtargetposition = transform.position + Random.insideUnitSphere * 5;
        if (NavMesh.CalculatePath(transform.position, testtargetposition, NavMesh.AllAreas, path))
        {
            bool isvalid = true;
            if (path.status != NavMeshPathStatus.PathComplete) isvalid = false;
            if (isvalid)
            {
                target = testtargetposition;
                routine = walking();
                StartCoroutine(routine);
            }
            else
            {
                SearchForATarget();
            }
        }
        else
        {
            SearchForATarget();
        }
    }
    private IEnumerator walking()
    {
        agent.SetDestination(target);
        while (Vector2.Distance(new Vector2(target.x, target.z), new Vector2(transform.position.x, transform.position.z)) > 1f)
        {
            /*animator.SetFloat(horizontale_trigger, horizontal);
            animator.SetFloat(verticale_trigger, vertical);

            if(target.x - transform.position.x > 0)
            {
                //rechts
                horizontal = 1;
            }
            else if (target.x - transform.position.x < 0)
            {
                //links
                horizontal = -1;
            }

            if (target.z - transform.position.z > 0)
            {
                //gaat naar voren
                vertical = 1;
            }
            else if (target.z - transform.position.z < 0)
            {
                //gaat naar achteren
                vertical = -1;
            }
            if(target.x - transform.position.x == 0 && target.z - transform.position.z == 0)
            {
                horizontal = 0;
                vertical = 0;
            }*/

            /*//calculate which animation state to play
            if(target.x - transform.position.x > 0)
            {
                //gaat naar rechts 
                spriterender.flipX = false;
                animator.SetBool(Beweeg_Rechts_Zin, true);
            }
            else if (target.x - transform.position.x < 0)
            {
                //gaat naar links
                spriterender.flipX = true;
                animator.SetBool(Beweeg_Rechts_Zin, true);
            }
            else
            {
                animator.SetBool(Beweeg_Rechts_Zin, false);
                if(target.z - transform.position.z > 0)
                {
                    //gaat naar voren
                    animator.SetBool(Beweeg_Voorwaards_Zin, true);
                }
                else if(target.z - transform.position.z < 0)
                {
                    //gaat naar achteren
                    animator.SetBool(Beweeg_Achterwaards_Zin, true);
                }
                else
                {
                    animator.SetBool(Beweeg_Achterwaards_Zin, false);
                    animator.SetBool(Beweeg_Voorwaards_Zin, false);
                }
            }*/
            yield return null;
        }
        target = transform.position;
        //animator.SetBool(Beweeg_Achterwaards_Zin, false);
        //animator.SetBool(Beweeg_Voorwaards_Zin, false);
        //animator.SetBool(Beweeg_Rechts_Zin, false);
        ChooseAction();
    }

    private void Chooserandomaction()
    {
        int randomseed = Random.Range(0, Overige_trigger.Count);
        routine = RandomAction(randomseed);
        StartCoroutine(routine);
    }

    private IEnumerator RandomAction(int animationnumber)
    {
        target = transform.position;
        animator.SetBool(Overige_trigger[animationnumber], true);
        yield return new WaitForSeconds(0.1f);
        float duration = animator.GetCurrentAnimatorClipInfo(0).Length;
        Debug.Log(duration);
        yield return new WaitForSeconds(duration);
        animator.SetBool(Overige_trigger[animationnumber], false);
        yield return new WaitForSeconds(0.5f);
        ChooseAction();
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////speler control
    /// </summary>

    private void PlayerControl()
    {
        if (npcMode == true)
        {
            StopCoroutine(routine);
            npcMode = false;
        }
        if (controller.isGrounded && velocity < 0)
        {
            velocity = -1f;
        }
        else
        {
            velocity += Gravity * gravityMultiplier * Time.deltaTime;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        animator.SetFloat(horizontale_trigger, horizontal);
        animator.SetFloat(verticale_trigger, vertical);

        Vector3 direction = new Vector3(horizontal, velocity, vertical).normalized;
        if (direction.magnitude >= 0.1f)
        {
            controller.Move(direction * loopsnelheid * Time.deltaTime);
        }
    }



}
