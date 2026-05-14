using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    public NPCData data;
    private Transform player;

    [Header("UI Settings")]
    public float showDistance = 60f;
    private TextMeshPro textMesh;

    [Header("AI Settings")]
    public float sightRange = 15f;
    public float wanderRadius = 30f;
    public float waitAtDestination = 3f;

    [Header("Combat Settings")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime = -10f;

    private NavMeshAgent agent;
    private float waitTimer;
    
    private enum AIState { Wander, Chase }
    private AIState currentState;

    void Start()
    {
        SetupColliderAndUI();
        FindPlayer();

        agent = gameObject.AddComponent<NavMeshAgent>();
        agent.speed = data.moveSpeed;
        agent.stoppingDistance = 1.5f;
        agent.acceleration = 20f;

        agent.baseOffset = 1.5f;

        agent.updateRotation = false;

        currentState = AIState.Wander;
   
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        UpdateUI();
        HandleAI();

        OrientTowardsMovement();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.transform.root.CompareTag("Player"))
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                PlayerStats pStats = collision.transform.root.GetComponent<PlayerStats>();
                if (pStats != null) pStats.TakeDamage(data.damage);

                TakeDamage(PlayerData.Damage);

                Vector3 pushDirection = (transform.position - collision.transform.position).normalized;
                pushDirection.y = 0;

                if (agent.isActiveAndEnabled) agent.velocity = pushDirection * 15f;

                Rigidbody pRb = collision.transform.root.GetComponent<Rigidbody>();
                if (pRb != null) pRb.AddForce(-pushDirection * 20f, ForceMode.Impulse);

                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        float reducedDamage = amount - (data.armor * 0.1f);
        reducedDamage = Mathf.Max(reducedDamage, 1f);

        data.hp -= reducedDamage;

        Debug.Log($"{data.npcName} has taken {reducedDamage} damage. Current HP: {data.hp}/{data.maxHp}");

        textMesh.text = GenerateUIText();

        if (data.hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{data.npcName} has died.");
        PlayerData.enemiesDefeated++;

        Destroy(gameObject);
    }

    private void HandleAI()
    {

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = false;

        if (distanceToPlayer <= sightRange)
        {
            Vector3 enemyEyes = transform.position + Vector3.up * 1.5f;
            Vector3 playerCenter = player.position + Vector3.up * 1.0f;

            Vector3 dirToPlayer = (playerCenter - enemyEyes).normalized;

            // Debug.DrawRay(enemyEyes, dirToPlayer * sightRange, Color.red);

            if (Physics.Raycast(enemyEyes, dirToPlayer, out RaycastHit hit, sightRange))
            {
                if (hit.collider.CompareTag("Player") || hit.collider.transform.root.CompareTag("Player"))
                {
                    canSeePlayer = true;
                }
            }
        }

        if (canSeePlayer)
        {
            currentState = AIState.Chase;
        }
        else
        {
            currentState = AIState.Wander;
        }


        switch (currentState)
        {
            case AIState.Wander:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    waitTimer += Time.deltaTime;

                    if (waitTimer >= waitAtDestination)
                    {
                        if (RandomNavSphere(transform.position, wanderRadius, -1, out Vector3 newPos))
                        {
                            agent.SetDestination(newPos);
                            waitTimer = 0f;
                        }
                    }
                }
                break;

            case AIState.Chase:
                agent.SetDestination(player.position);
                break;
        }
    }

    private void OrientTowardsMovement()
    {
     
        Vector3 moveDirection = agent.desiredVelocity;
        moveDirection.y = 0f;

        if (moveDirection.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            targetRotation *= Quaternion.Euler(0, 90f, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public static bool RandomNavSphere(Vector3 origin, float dist, int layermask, out Vector3 result)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
  
        if (NavMesh.SamplePosition(randDirection, out navHit, dist, layermask))
        {
            result = navHit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }


    private void OrientTowardsPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            targetRotation *= Quaternion.Euler(0, 90f, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private void UpdateUI()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= showDistance)
        {
            textMesh.enabled = true;
            if (Camera.main != null)
            {
                textMesh.transform.LookAt(textMesh.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            }
        }
        else
        {
            textMesh.enabled = false;
        }
    }

    private void SetupColliderAndUI()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        if (col == null)
        {
            col = gameObject.AddComponent<SphereCollider>();
            col.radius = 0.6f;
            col.isTrigger = false;
        }

        GameObject textObj = new GameObject("EnemyUI");
        textObj.transform.SetParent(this.transform);
        textObj.transform.localPosition = new Vector3(0, 1.5f, 0);

        textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.fontSize = 3.5f;
        textMesh.outlineWidth = 0.2f;
        textMesh.outlineColor = Color.black;

        textMesh.text = GenerateUIText();
        textMesh.enabled = false;
    }

    void FindPlayer()
    {
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    string GenerateUIText()
    {
        string classColor = "#FFFFFF";
        if (data.npcClass == NPCClass.Warrior) classColor = "#FF8800";
        if (data.npcClass == NPCClass.Paladin) classColor = "#FFFF00";
        if (data.npcClass == NPCClass.Assassin) classColor = "#FF0000";
        if (data.npcClass == NPCClass.Rogue) classColor = "#A020F0";

        string t = $"<color={classColor}>{data.npcName}</color>\n<size=70%>[{data.npcClass}]</size>\n";
        t += $"<size=80%><color=#FF4444>HP:</color> {data.hp}/{data.maxHp} | <color=#AAAAAA>AR:</color> {data.armor}\n";
        t += $"<color=#FFaa00>DMG:</color> {data.damage} | <color=#00FF00>SPD:</color> {data.moveSpeed}</size>";
        return t;
    }
}