using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public enum UnitType
{
    Soldier, Villager,Build,Archer
}
public class Unit : MonoBehaviour
{
    public enum UnitState
    {
        nothing, onFarming,onMoving,onAttacking,onBuilding
    }
    public UnitState state;
    public UnitState Laststate;

    [SerializeField] public UnitType Type;
    private NavMeshAgent agent;
    private Rigidbody rb;
    public Animator animator;
    [SerializeField] GameObject selectCircle;
    SphereCollider trigger;
    private GameObject centerVillage;
   public bool canMove;
     public Vector3 MovePoint;
    GameManager gameManager;
    public float UnitHp;
    public float MaxUnitHp;

    #region Villager Stuff
    [Header("Resources Info")]
    public GameObject currentResourceFarm;
    private int dmgInResource = 10;
    public int farmHitCount = 0;
    [Header("Villager")]
    [SerializeField] int farmHitCountMax;
    public bool depositeResource;
    public List<GameObject> resourceFarmList;
    public bool readyToFarm;
    public GameObject currentBlueprint;
    ResourceTypes lastFarm;
    [Header("Inventory")]
    [HideInInspector] public int WoodAmout;
    [HideInInspector] public int FoodAmout;
    [HideInInspector] public int IronAmout;
    [HideInInspector] public int GoldAmout;
    #endregion
    #region Soldier Stuff
    #region Enemy
    [Header("Soldier")]
    [SerializeField] WeaponDmg weapon;
    [SerializeField] public float WeaponDmg;
     public List<GameObject> allAllyAround;
    public GameObject currentAllyToFight;
    #endregion
    #region Ally
    public List<Unit> allEnemyAround;
    public GameObject currentEnemyToFight;
    #endregion
    #endregion
    [Header("Archer")]
    [SerializeField] float AttackRange;
    [SerializeField] BowDmg bow;
    bool villageAttack;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        gameManager = GameObject.FindAnyObjectByType<GameManager>();
        centerVillage = GameObject.Find("VillageCenter");
        state = UnitState.nothing;
        trigger = GetComponent<SphereCollider>();
        trigger.enabled = false;
        UnitHp = MaxUnitHp;
        if ((gameObject.tag == "Enemy" || gameObject.tag == "Ally") && 
            (Type == UnitType.Soldier || Type == UnitType.Archer))
        {
            MoveToVillageCenter();
            trigger.enabled = true;
        }
    }

    private void Update()
    {
        if (UnitHp > 0)
        {
            if (Type == UnitType.Villager)
            {
                VillagerBehavier();
            }
            else if (Type == UnitType.Soldier)
            {
                if (gameObject.tag == "Enemy")
                {
                    EnemyBehaviour();
                }
                if (gameObject.tag == "Ally")
                {
                    AllyBehavior();
                }
            }
            else if(Type == UnitType.Archer)
            {
                if (gameObject.tag == "Enemy")
                {
                    EnemyArcherBehaviour();
                }
                if (gameObject.tag == "Ally")
                {
                    ArcherAllyBehavier();
                }
            }
        }
        else if(UnitHp <= 0)
        {
            DeadAnimation();
        }
    }
    public void MoveToPoint(Vector3 point)
    {
        MovePoint = point;
        canMove = true;
        state = UnitState.nothing;
        AttackAnimation(false);
        currentEnemyToFight = null;

    }
    public void StopMoving()
    {
        agent.isStopped = true;
    }

    void Move()
    {
        if (canMove && !gameManager.InConstructMode)
        {
            agent.SetDestination(MovePoint);

            if (Type == UnitType.Archer || Type == UnitType.Soldier)
            {
                if (agent.remainingDistance == 0 && (allAllyAround.Count != 0 || allEnemyAround.Count != 0))
                {
                    FindClosestEnemyToFight();
                    state = UnitState.onAttacking;
    
                }
            }
        }
        


    }
    public void StateOfSelectedState(bool isSelected)
    {
        if (isSelected)
        {
            selectCircle.SetActive(true);
        }
        else
        {
            selectCircle.SetActive(false);
        }
    }
    public void LocomotionAnimation()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }
    void DeadAnimation()
    {
        animator.SetBool("Attacking", false);
        animator.SetTrigger("Dead");
    }
    IEnumerator Die()
    {
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<NavMeshObstacle>().enabled = false;
        agent.isStopped = true;
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    private void OnTriggerStay(Collider other)
    {
        if (trigger.enabled)
        {
            if (Type == UnitType.Villager)
            {
                if (currentResourceFarm != null)
                {
                    if (other.transform.gameObject.GetComponent<ResourceType>() != null &&
                        other.transform.gameObject.GetComponent<ResourceType>().type == currentResourceFarm.GetComponent<ResourceType>().type)
                    {
                        if (!resourceFarmList.Contains(other.transform.gameObject) &&
                            other.gameObject != currentResourceFarm &&
                            other.gameObject.GetComponent<ResourceType>().ResourceHP == 10) resourceFarmList.Add(other.transform.gameObject);
                    }
                }
            }
            else if(Type == UnitType.Soldier)
            {
                if(gameObject.tag == "Enemy")
                {
                    if (other.gameObject.GetComponent<Unit>() != null)
                    {
                        if (other.gameObject.tag == "Ally" && (other.gameObject.GetComponent<Unit>().Type == UnitType.Soldier ||
                            other.GetComponent<Unit>().Type == UnitType.Archer))
                        {
                            if (!allAllyAround.Contains(other.gameObject) && other.GetComponent<Unit>().UnitHp > 0)
                            {
                                allAllyAround.Add(other.gameObject);
                                currentAllyToFight = other.gameObject;
                                state = UnitState.onAttacking;
                            }
                        }
                    }
                    else if(other.gameObject.GetComponent<Build>() != null)
                    {
                        if (other.gameObject.GetComponent<Build>().buildtype == BuildType.Tower)
                        {
                            if (!allAllyAround.Contains(other.gameObject) && other.GetComponent<Build>().BuildHp > 0)
                            {
                                allAllyAround.Add(other.gameObject);
                                currentAllyToFight = other.gameObject;
                                state = UnitState.onAttacking;
                            }
                        }
                        
                    }
                }
                else if(gameObject.tag == "Ally")
                {
                    if (other.gameObject.tag == "Enemy" && (other.gameObject.GetComponent<Unit>().Type == UnitType.Soldier ||
                        other.GetComponent<Unit>().Type == UnitType.Archer))
                    {
                        if (!allEnemyAround.Contains(other.GetComponent<Unit>()) && other.GetComponent<Unit>().UnitHp > 0)
                        {
                            allEnemyAround.Add(other.GetComponent<Unit>());
                            currentEnemyToFight = other.gameObject;
                            state = UnitState.onAttacking;
                        }
                    }
                }
            }
            else if(Type == UnitType.Archer)
            {
                if (gameObject.tag == "Enemy")
                {
                    if (other.gameObject.GetComponent<Unit>() != null)
                    {
                        if (other.gameObject.tag == "Ally" && (other.gameObject.GetComponent<Unit>().Type == UnitType.Soldier ||
                        other.gameObject.GetComponent<Unit>().Type == UnitType.Archer))
                        {
                            if (!allAllyAround.Contains(other.gameObject) && other.GetComponent<Unit>().UnitHp > 0)
                            {
                                allAllyAround.Add(other.gameObject);
                                currentAllyToFight = other.gameObject;
                                state = UnitState.onAttacking;
                            }
                        }
                    }
                    else if (other.gameObject.GetComponent<Build>() != null)
                    {
                        if (other.gameObject.GetComponent<Build>().buildtype == BuildType.Tower)
                        {
                            if (!allAllyAround.Contains(other.gameObject) && other.GetComponent<Build>().BuildHp > 0)
                            {
                                allAllyAround.Add(other.gameObject);
                                currentAllyToFight = other.gameObject;
                                state = UnitState.onAttacking;
                            }
                        }

                    }
                }
                else if (gameObject.tag == "Ally")
                {
                    if (other.gameObject.tag == "Enemy" && (other.gameObject.GetComponent<Unit>().Type == UnitType.Archer ||
                        other.gameObject.GetComponent<Unit>().Type == UnitType.Soldier))
                    {
                        if (!allEnemyAround.Contains(other.GetComponent<Unit>()) && other.GetComponent<Unit>().UnitHp > 0)
                        {
                            allEnemyAround.Add(other.GetComponent<Unit>());
                            currentEnemyToFight = other.gameObject;
                            state = UnitState.onAttacking;
                        }
                    }
                }
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (Type == UnitType.Soldier)
        {
            if (gameObject.tag == "Enemy")
            {
                if (other.gameObject.GetComponent<Unit>() != null)
                {
                    if (other.gameObject.tag == "Ally" && (other.gameObject.GetComponent<Unit>().Type == UnitType.Soldier ||
                        other.gameObject.GetComponent<Unit>().Type == UnitType.Archer))
                    {
                        if (allAllyAround.Contains(other.gameObject))
                        {
                            allAllyAround.Remove(other.gameObject);
                        }
                    }
                }
                else if(other.gameObject.GetComponent<Build>() != null)
                {
                    if(other.gameObject.GetComponent<Build>().buildtype == BuildType.Tower)
                    {
                        if (allAllyAround.Contains(other.gameObject))
                        {
                            allAllyAround.Remove(other.gameObject);
                        }
                    }
                }
                    
            }
            else if (gameObject.tag == "Ally")
            {
                if (other.gameObject.tag == "Enemy" && (other.gameObject.GetComponent<Unit>().Type == UnitType.Soldier ||
                    other.gameObject.GetComponent<Unit>().Type == UnitType.Archer))
                {
                    if (allEnemyAround.Contains(other.GetComponent<Unit>()))
                    {
                        allEnemyAround.Remove(other.GetComponent<Unit>());
                    }
                }
            }
        }
        else if (Type == UnitType.Archer)
        {
            if (gameObject.tag == "Enemy")
            {
                if (other.gameObject.GetComponent<Unit>() != null)
                {
                    if (other.gameObject.tag == "Ally" && (other.gameObject.GetComponent<Unit>().Type == UnitType.Soldier ||
                    other.gameObject.GetComponent<Unit>().Type == UnitType.Archer))
                    {
                        if (allAllyAround.Contains(other.gameObject))
                        {
                            allAllyAround.Remove(other.gameObject);
                        }
                    }
                }
                else if (other.gameObject.GetComponent<Build>() != null)
                {
                    if (other.gameObject.GetComponent<Build>().buildtype == BuildType.Tower)
                    {
                        if (allAllyAround.Contains(other.gameObject))
                        {
                            allAllyAround.Remove(other.gameObject);
                        }
                    }
                }
            }
            else if (gameObject.tag == "Ally")
            {
                if (other.gameObject.tag == "Enemy" && (other.gameObject.GetComponent<Unit>().Type == UnitType.Archer ||
                    other.gameObject.GetComponent<Unit>().Type == UnitType.Soldier))
                {
                    if (allEnemyAround.Contains(other.GetComponent<Unit>()))
                    {
                        allEnemyAround.Remove(other.GetComponent<Unit>());
                    }
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<VillageCenter>() != null)
        {
            AttackVillagerCenter();
        }
    }
    #region Soldier Methods
    #region Enemy
    void AttackVillagerCenter()
    {
        villageAttack = true;
        AttackAnimation(true);
    }
    void EnemyBehaviour()
    {
        if (state == UnitState.onMoving)
        {
            Move();
            LocomotionAnimation();
        }
        else if (state == UnitState.onAttacking)
        {
            if (currentAllyToFight != null)
            {
                MoveToAttack(currentAllyToFight.transform.position);
                if (currentAllyToFight.GetComponent<Unit>() != null)
                {
                    if (currentAllyToFight.GetComponent<Unit>().UnitHp < 
                        currentAllyToFight.GetComponent<Unit>().MaxUnitHp)
                    {
                        allAllyAround.Remove(currentAllyToFight);
                    }
                    if (currentAllyToFight.GetComponent<Unit>().UnitHp <= 0)
                    {
                        currentAllyToFight = null;
                        if (allAllyAround.Count > 0)
                        {
                            FindClosestAllyToFight();
                        }
                        else
                        {
                            MoveToVillageCenter();
                            AttackAnimation(false);
                        }
                    }
                }
                else if(currentAllyToFight.GetComponent<Build>() != null)
                {
                    if (currentAllyToFight.GetComponent<Build>().BuildHp <
                       currentAllyToFight.GetComponent<Build>().MaxBuildHp)
                    {
                        allAllyAround.Remove(currentAllyToFight);
                    }
                    if (currentAllyToFight.GetComponent<Build>().BuildHp <= 0)
                    {
                        currentAllyToFight = null;
                        if (allAllyAround.Count > 0)
                        {
                            FindClosestAllyToFight();
                        }
                        else
                        {
                            MoveToVillageCenter();
                            AttackAnimation(false);
                        }
                    }
                }


            }
            else
            {
                MoveToVillageCenter();
                AttackAnimation(false);
            }
        }
    }
    void Attack()
    {
        if (weapon != null)
        {
            if (weapon.hit)
            {
                if (gameObject.tag == "Enemy")
                {
                    if (currentAllyToFight != null)
                    {
                        if (currentAllyToFight.GetComponent<Unit>() != null)
                        {
                            if (currentAllyToFight.GetComponent<Unit>().UnitHp > 0)
                            {
                                currentAllyToFight.GetComponent<Unit>().UnitHp -= WeaponDmg;
                                weapon.hit = false;
                            }

                        }
                        else if (currentAllyToFight.GetComponent<Build>() != null)
                        {
                            if (currentAllyToFight.GetComponent<Build>().BuildHp > 0)
                            {
                                currentAllyToFight.GetComponent<Build>().BuildHp -= WeaponDmg;
                                weapon.hit = false;
                            }
                        }
                    }
                    if(villageAttack)
                    {
                        if (centerVillage.GetComponent<Build>().BuildHp > 0)
                        {
                            centerVillage.GetComponent<Build>().BuildHp -= WeaponDmg;
                            weapon.hit = false;
                        }
                    }
                }
                else if(gameObject.tag == "Ally")
                {
                    
                    if (currentEnemyToFight != null)
                    {
                        if (currentEnemyToFight.GetComponent<Unit>().UnitHp > 0)
                        {
                            currentEnemyToFight.GetComponent<Unit>().UnitHp -= WeaponDmg;
                            weapon.hit = false;
                        }
                    }
                }
            }
        }
    }
    void AttackAnimation(bool state)
    {
        agent.isStopped = state;
        animator.SetBool("Attacking", state);

        if(currentAllyToFight != null)
        transform.rotation = Quaternion.LookRotation(currentAllyToFight.transform.position - transform.position);
        if(currentEnemyToFight != null)
        transform.rotation = Quaternion.LookRotation(currentEnemyToFight.transform.position - transform.position);

    }
    void MoveToAttack(Vector3 current)
    {
        MovePoint = current - new Vector3(1.5f,0f,1.5f);
        //Debug.Log(Vector3.Distance(gameObject.transform.position, currentAllyToFight.transform.position));
        if (currentAllyToFight.GetComponent<Unit>() != null)
        {
            if (Vector3.Distance(gameObject.transform.position, currentAllyToFight.transform.position) < 2.2f)
            {
                AttackAnimation(true);
            }
            else
            {
                FindClosestAllyToFight();
                AttackAnimation(false);
                Move();
                LocomotionAnimation();
            }
        }
        else if(currentAllyToFight.GetComponent<Build>() != null)
        {
            if (Vector3.Distance(gameObject.transform.position, currentAllyToFight.transform.position) < 3.7f)
            {
                AttackAnimation(true);
            }
            else
            {
                FindClosestAllyToFight();
                AttackAnimation(false);
                Move();
                LocomotionAnimation();
            }
        }
    }
    void MoveToVillageCenter()
    {
        MovePoint = centerVillage.transform.position;
        canMove = true;
        state = UnitState.onMoving;
    }
    void FindClosestAllyToFight()
    {
        float closestEnemy = 10000f;
        if(allAllyAround != null)
        {
            foreach(var other in allAllyAround)
            {
                if (other != null)
                {
                    if (Vector3.Distance(transform.position, other.transform.position) < closestEnemy)
                    {
                        closestEnemy = Vector3.Distance(transform.position, other.transform.position);
                        currentAllyToFight = other.gameObject;
                    }
                }
            }
        }
        else
        {
            state = UnitState.onMoving;
        }
    }
    #endregion
    #region Ally
    void AllyBehavior()
    {
        if (state == UnitState.nothing)
        {
            Move();
            LocomotionAnimation();
        }
        else if (state == UnitState.onAttacking)
        {
            if (currentEnemyToFight != null)
            {
                MoveToAttackEnemy(currentEnemyToFight.transform.position);

                if (currentEnemyToFight.GetComponent<Unit>().UnitHp <
                    currentEnemyToFight.GetComponent<Unit>().MaxUnitHp)
                {
                    allEnemyAround.Remove(currentEnemyToFight.GetComponent<Unit>());
                }
                if (currentEnemyToFight.GetComponent<Unit>().UnitHp <= 0)
                {
                    currentEnemyToFight = null;
                    if (allEnemyAround.Count > 0)
                    {
                        FindClosestEnemyToFight();
                    }
                    else
                    {
                        Move();
                        AttackAnimation(false);
                    }
                }
            }
            else
            {
                state = UnitState.nothing;
                Move();
                AttackAnimation(false);
            }
        }
    }
    void MoveToAttackEnemy(Vector3 current)
    {
        MovePoint = current - new Vector3(1.5f, 0f, 1.5f);
        //Debug.Log(Vector3.Distance(gameObject.transform.position, currentAllyToFight.transform.position));
        if (Vector3.Distance(gameObject.transform.position, currentEnemyToFight.transform.position) < 2.2f)
        {
            AttackAnimation(true);
        }
        else
        {
            FindClosestEnemyToFight();
            AttackAnimation(false);
            Move();
            LocomotionAnimation();
        }
    }
    void FindClosestEnemyToFight()
    {
        float closestEnemy = 10000f;
        if (allEnemyAround != null)
        {
            foreach (var other in allEnemyAround)
            {
                if (other != null)
                {
                    if (Vector3.Distance(transform.position, other.transform.position) < closestEnemy)
                    {
                        closestEnemy = Vector3.Distance(transform.position, other.transform.position);
                        currentEnemyToFight = other.gameObject;
                    }
                }
            }
        }
        else
        {
            state = UnitState.nothing;
        }
    }
    public void MoveToEnemy(GameObject enemy)
    {
        state = UnitState.onAttacking;
        canMove = true;
        currentEnemyToFight = enemy;    
    }
    #endregion
    #endregion
    #region Archer Methods
    #region Ally
    void ArcherAllyBehavier()
    {
        if (state == UnitState.nothing)
        {
            Move();
            LocomotionAnimation();
           
        }
        else if (state == UnitState.onAttacking)
        {
            if (currentEnemyToFight != null)
            {
                ArcherAttackRangeCheck(currentEnemyToFight.transform.position);

                if (currentEnemyToFight.GetComponent<Unit>().UnitHp <
                    currentEnemyToFight.GetComponent<Unit>().MaxUnitHp)
                {
                    allEnemyAround.Remove(currentEnemyToFight.GetComponent<Unit>());
                }
                if (currentEnemyToFight.GetComponent<Unit>().UnitHp <= 0)
                {
                    currentEnemyToFight = null;
                    if (allEnemyAround.Count > 0)
                    {
                        FindClosestEnemyToFight();
                    }
                    else
                    {
                        Move();
                        AttackAnimation(false);
                    }
                }
            }
            else
            {
                state = UnitState.nothing;
                Move();
                AttackAnimation(false);
            }
        }
    }
    void ArcherAttackRangeCheck(Vector3 current)
    {
        //Debug.Log(Vector3.Distance(gameObject.transform.position, currentAllyToFight.transform.position));
        if (Vector3.Distance(gameObject.transform.position, currentEnemyToFight.transform.position) < AttackRange)
        {
            MovePoint = transform.position;
            AttackAnimation(true);
        }
        else
        {
            FindClosestEnemyToFight();
            MovePoint = current;
            AttackAnimation(false);
            Move();
            LocomotionAnimation();
        }
    }
    void AttackBow()
    {
        if (villageAttack)
            bow.arrowTarget = centerVillage;
        else
        {
            if (tag == "Ally")
                bow.arrowTarget = currentEnemyToFight;
            else if (tag == "Enemy")
                bow.arrowTarget = currentAllyToFight;
        }

        bow.CorrectArrowLogic();
    }
    #endregion
    #region Enemy
    void EnemyArcherAttackRangeCheck(Vector3 current)
    {
        //Debug.Log(Vector3.Distance(gameObject.transform.position, centerVillage.transform.position));
        if (Vector3.Distance(gameObject.transform.position, centerVillage.transform.position) > AttackRange)
        {
            //Debug.Log(Vector3.Distance(gameObject.transform.position, currentAllyToFight.transform.position));
            if (Vector3.Distance(gameObject.transform.position, currentAllyToFight.transform.position) < AttackRange)
            {
                MovePoint = transform.position;
                AttackAnimation(true);
            }
            else
            {
                FindClosestAllyToFight();
                MovePoint = current;
                AttackAnimation(false);
                Move();
                LocomotionAnimation();
            }
        }
    }
    void EnemyArcherBehaviour()
    {
        if (state == UnitState.onMoving)
        {
            Move();
            LocomotionAnimation();

            if (Vector3.Distance(gameObject.transform.position, centerVillage.transform.position) < AttackRange)
            {
                villageAttack = true;
                MovePoint = transform.position;
                AttackAnimation(true);
            }
        }
        else if (state == UnitState.onAttacking)
        {
            if (currentAllyToFight != null)
            {
                EnemyArcherAttackRangeCheck(currentAllyToFight.transform.position);
                if (currentAllyToFight.GetComponent<Unit>() != null)
                {
                    if (currentAllyToFight.GetComponent<Unit>().UnitHp < MaxUnitHp)
                    {
                        allAllyAround.Remove(currentAllyToFight);
                    }
                    if (currentAllyToFight.GetComponent<Unit>().UnitHp <= 0)
                    {
                        currentAllyToFight = null;
                        if (allAllyAround.Count > 0)
                        {
                            FindClosestAllyToFight();
                        }
                        else
                        {
                            MoveToVillageCenter();
                            AttackAnimation(false);
                        }
                    }
                }
                else if(currentAllyToFight.GetComponent<Build>() != null)
                {
                    if (currentAllyToFight.GetComponent<Build>().BuildHp <
                        currentAllyToFight.GetComponent<Build>().MaxBuildHp)
                    {
                        allAllyAround.Remove(currentAllyToFight);
                    }
                    if (currentAllyToFight.GetComponent<Build>().BuildHp <= 0)
                    {
                        currentAllyToFight = null;
                        if (allAllyAround.Count > 0)
                        {
                            FindClosestAllyToFight();
                        }
                        else
                        {
                            MoveToVillageCenter();
                            AttackAnimation(false);
                        }
                    }
                }
            }
            else
            {
                MoveToVillageCenter();
                AttackAnimation(false);
            }
        }
    }
    #endregion
    #endregion
    #region Resources Methods and Villager Methods
    public void MoveToBuild(Vector3 point)
    {
        MovePoint = point;
        canMove = true;
    }
    void Build()
    {
        if(currentBlueprint != null)
        {
            if(currentBlueprint.transform.position.y >= currentBlueprint.GetComponent<Build>().EndYPosition)
            {
                currentBlueprint.transform.position = new Vector3(currentBlueprint.transform.position.x, 
                    currentBlueprint.GetComponent<Build>().EndYPosition, currentBlueprint.transform.position.z);
                currentBlueprint.GetComponent<Build>().onBuilding = false;
                animator.SetBool("Building", false);
                state = UnitState.nothing;
                currentBlueprint = null;
                canMove = false;
                agent.isStopped = false;
                agent.SetDestination(transform.position);
            }
            else
            {
                currentBlueprint.transform.position += new Vector3(0, 0.5f, 0);
            }
        }
    }
    void VillagerBehavier()
    {
        if (state == UnitState.nothing)
        {
            Move();
            LocomotionAnimation();
            if (Laststate == UnitState.onFarming)
            {
                depositeResource = false;
                readyToFarm = false;
                if (currentResourceFarm != null) FarmingAnims(currentResourceFarm.GetComponent<ResourceType>().type, false);
                RemoveWorkerNumber(currentResourceFarm);
                if(currentResourceFarm.GetComponent<Build>().buildtype == BuildType.FoodFarm)
                    currentResourceFarm.GetComponent<Build>().isUsed = false;
                currentResourceFarm = null;
                Laststate = UnitState.nothing;
                agent.isStopped = false;
                trigger.enabled = false;
            }
            animator.SetBool("Building", false);
        }
        else if (state == UnitState.onFarming)
        {
            if (currentResourceFarm != null)
            {
                addWorkerNumber(currentResourceFarm);
                if (currentResourceFarm.GetComponent<ResourceType>() != null)
                {
                    if (currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Wood)
                    {
                        FarmWood();
                    }
                    else if (currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Food)
                    {
                        FarmFood();
                    }
                    else if (currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Iron)
                    {
                        FarmGold();
                    }
                    else if (currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Gold)
                    {
                        FarmGold();
                    }
                }
            }
            else
            {
                if (resourceFarmList.Count == 0)
                {
                    state = UnitState.nothing;
                }
            }
            animator.SetBool("Building", false);

        }
        else if(state == UnitState.onBuilding)
        {
            if(currentBlueprint != null)
            {
                if (currentBlueprint.GetComponent<Build>() != null)
                {
                    if (currentBlueprint.GetComponent<Build>().buildtype == BuildType.FoodFarm)
                    {
                        if (transform.position.x == currentBlueprint.transform.position.x &&
                            transform.position.z == currentBlueprint.transform.position.z)
                        {
                            animator.SetBool("Building", true);
                            canMove = false;
                        }
                    }
                }
            }
            if (!animator.GetBool("Building"))
            {
                Move();
                LocomotionAnimation();
            }
            else
            {
                agent.isStopped = true;
            }
        }
    }
    void addWorkerNumber(GameObject current)
    {
        switch (current.GetComponent<ResourceType>().type)
        {
            case ResourceTypes.Wood:
                {
                    if (!gameManager.WoodWorkers.Contains(this)) { gameManager.WoodWorkers.Add(this);
                        lastFarm = ResourceTypes.Wood;
                    }
                    break;
                }
            case ResourceTypes.Food:
                {
                    if (!gameManager.FoodWorkers.Contains(this)) { gameManager.FoodWorkers.Add(this);
                        lastFarm = ResourceTypes.Food;
                    }
                    break;
                }
            case ResourceTypes.Gold:
                {
                    if (!gameManager.GoldWorkers.Contains(this))
                    {
                        gameManager.GoldWorkers.Add(this);
                        lastFarm = ResourceTypes.Gold;
                    }
                    break;
                }
            case ResourceTypes.Iron:
                {
                    if (!gameManager.IronWorkers.Contains(this))
                    {
                        gameManager.IronWorkers.Add(this);
                        lastFarm = ResourceTypes.Iron;
                    }
                    break;
                }
        }
    }
    void RemoveWorkerNumber(GameObject current)
    {
        if (current != null)
        {
            switch (current.GetComponent<ResourceType>().type)
            {
                case ResourceTypes.Wood:
                    {
                        if (gameManager.WoodWorkers.Contains(this)) gameManager.WoodWorkers.Remove(this);
                        break;
                    }
                case ResourceTypes.Food:
                    {
                        if (gameManager.FoodWorkers.Contains(this)) gameManager.FoodWorkers.Remove(this);
                        break;
                    }
                case ResourceTypes.Gold:
                    {
                        if (gameManager.GoldWorkers.Contains(this)) gameManager.GoldWorkers.Remove(this);
                        break;
                    }
                case ResourceTypes.Iron:
                    {
                        if (gameManager.IronWorkers.Contains(this)) gameManager.IronWorkers.Remove(this);

                        break;
                    }
            }
        }
        else
        {
            switch(lastFarm)
            {
                case ResourceTypes.Wood:
                    {
                        if (gameManager.WoodWorkers.Contains(this)) { gameManager.WoodWorkers.Remove(this);
                            lastFarm = ResourceTypes.Nothing;
                        };
                        break;
                    }
                case ResourceTypes.Food:
                    {
                        if (gameManager.FoodWorkers.Contains(this)) { gameManager.FoodWorkers.Remove(this);
                            lastFarm = ResourceTypes.Nothing;
                        };
                        break;
                    }
                case ResourceTypes.Gold:
                    {
                        if (gameManager.GoldWorkers.Contains(this)) { gameManager.GoldWorkers.Remove(this);
                            lastFarm = ResourceTypes.Nothing;
                        }
                        break;
                    }
                case ResourceTypes.Iron:
                    {
                        if (gameManager.IronWorkers.Contains(this)) { gameManager.IronWorkers.Remove(this);
                            lastFarm = ResourceTypes.Nothing;
                        }

                        break;
                    }
            }
        }
    }
    void FarmFood()
    {
        if (!depositeResource)
        {
            if (!readyToFarm)
            {
                ReturnToResource();
                LocomotionAnimation();
            }
            else
            {
                if (farmHitCount < farmHitCountMax)
                {
                    FarmingAnims(currentResourceFarm.GetComponent<ResourceType>().type, true);
                }
                else
                {
                    FarmingAnims(currentResourceFarm.GetComponent<ResourceType>().type, false);
                    depositeResource = true;
                    readyToFarm = false;
                    farmHitCount = 0;
                }
            }
        }
        else
        {
            LocomotionAnimation();
            MoveToCenterVillage();
        }
    }
    void FarmGold()
    {
        if (!depositeResource)
        {
            if (!readyToFarm)
            {
                ReturnToResource();
                LocomotionAnimation();
            }
            else
            {
                if (farmHitCount < farmHitCountMax)
                {
                    if (currentResourceFarm.GetComponent<ResourceType>().ResourceHP <= 0)
                    {
                        trigger.enabled = true;
                        FarmingAnims(currentResourceFarm.GetComponent<ResourceType>().type, false);
                        FindNewResourceToFarm();
                    }
                    else
                    {
                        FarmingAnims(currentResourceFarm.GetComponent<ResourceType>().type, true);
                    }
                }
                else
                {
                    FarmingAnims(currentResourceFarm.GetComponent<ResourceType>().type, false);
                    depositeResource = true;
                    farmHitCount = 0;
                }
            }
        }
        else
        {
            if (currentResourceFarm.GetComponent<ResourceType>().ResourceHP <= 0)
            {
                FindNewResourceToFarm();
            }
            trigger.enabled = false;
            MoveToCenterVillage();
        }
    }
    void FarmWood()
    {
            if (!depositeResource)
            {
                if (!readyToFarm)
                {
                    ReturnToResource();
                    LocomotionAnimation();
                }
                else
                {
                    if (farmHitCount < farmHitCountMax)
                    {
                        if (currentResourceFarm.GetComponent<ResourceType>().ResourceHP <= 0)
                        {
                            trigger.enabled = true;
                            FarmingAnims(currentResourceFarm.GetComponent<ResourceType>().type, false);
                            FindNewResourceToFarm();
                        }
                        else
                        {
                            FarmingAnims(currentResourceFarm.GetComponent<ResourceType>().type, true);
                        }

                    }
                    else
                    {
                        FarmingAnims(currentResourceFarm.GetComponent<ResourceType>().type, false);
                        depositeResource = true;
                        farmHitCount = 0;
                    }

                }
            }
            else
            {
                if (currentResourceFarm.GetComponent<ResourceType>().ResourceHP <= 0)
                {
                    FindNewResourceToFarm();
                }
                trigger.enabled = false;
                MoveToCenterVillage();
            }
    }
    void FarmingAnims(ResourceTypes type,bool state)
    {
        if(type == ResourceTypes.Wood)
        {
            animator.SetBool("Wood", state);
        }
        else if(type == ResourceTypes.Food)
        {
            animator.SetBool("Food", state);
        }
        else if(type == ResourceTypes.Gold || type == ResourceTypes.Iron)
        {
            animator.SetBool("Mining", state);
        }

        if (currentResourceFarm != null)
        {
            if (currentResourceFarm.GetComponent<ResourceType>().type != ResourceTypes.Food)
                transform.rotation = Quaternion.LookRotation(currentResourceFarm.transform.position - transform.position);
        }

    }
    void FindNewResourceToFarm()
    {
        if (resourceFarmList.Count > 0)
        {
            float closeResource = 10000f;
            GameObject closestResource = null;
            foreach (GameObject resource in resourceFarmList)
            {
               if (Vector3.Distance(transform.position, resource.transform.position) < closeResource)
               {
                   closeResource = Vector3.Distance(transform.position, resource.transform.position);
                   closestResource = resource;
               }
            }
            currentResourceFarm = closestResource;
            readyToFarm = false;
            resourceFarmList.Clear();
        }
    } 
   
    void MoveToCenterVillage()
    {
        agent.isStopped = false;
        agent.SetDestination(centerVillage.transform.position);
    }
   public void ReturnToResource()
    {
        agent.isStopped = false;
        if (currentResourceFarm != null)
        {
            if(currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Wood) 
                agent.SetDestination(currentResourceFarm.transform.position);
            else if (currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Food) 
                agent.SetDestination(currentResourceFarm.GetComponent<ResourceType>().farmSpot.transform.position);
            else if(currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Gold)
                agent.SetDestination(currentResourceFarm.transform.position);
            else if (currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Iron)
                agent.SetDestination(currentResourceFarm.transform.position);

        }
    }
    public void PickUpResources(GameObject current)
    {
        currentResourceFarm = current;
        state = UnitState.onFarming;
        Laststate = UnitState.onFarming;
    }

    void DealDmgToResources()
    {
        if (currentResourceFarm != null)
        {
            if (currentResourceFarm.GetComponent<ResourceType>() != null)
            {
                if (currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Wood)
                {
                    currentResourceFarm.GetComponent<ResourceType>().ResourceHP -= dmgInResource;
                    farmHitCount++;
                    WoodAmout += 20;
                }
                else if (currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Food)
                { 
                    farmHitCount++;
                    FoodAmout += 20;
                }
                else if (currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Gold)
                {
                    currentResourceFarm.GetComponent<ResourceType>().ResourceHP -= dmgInResource;   
                    farmHitCount++;
                    GoldAmout += 20;
                }
                else if (currentResourceFarm.GetComponent<ResourceType>().type == ResourceTypes.Iron)
                {
                    currentResourceFarm.GetComponent<ResourceType>().ResourceHP -= dmgInResource;
                    farmHitCount++;
                    IronAmout += 20;
                }
            }
        }
        
    }
    #endregion

}
