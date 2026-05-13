using UnityEngine;
using TMPro;

public class NPCController : MonoBehaviour
{
    public NPCData data;
    private Transform player;

    private TextMeshPro textMesh;
    private float showDistance = 50f;

    void Start()
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

        FindPlayer();
    }

    void FindPlayer()
    {
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj != null) player = pObj.transform;
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

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

        Vector3 directionToPlayer = player.position - transform.position;

        directionToPlayer.y = 0f;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            targetRotation *= Quaternion.Euler(0, 90f, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
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