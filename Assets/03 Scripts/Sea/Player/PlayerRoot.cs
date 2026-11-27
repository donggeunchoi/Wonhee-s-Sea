using UnityEngine;

public class PlayerRoot : MonoBehaviour
{
    public Rigidbody2D Rb { get; private set; }

    public PlayerInputHandler Input { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public DungeonCombatInput CombatInput { get; private set; }
    public PlayerAttackController Attack { get; private set; }
    public PlayerInventory Inventory { get; private set; }
    public PlayerEquipment Equipment { get; private set; }
    public PlayerStats Stats { get; private set; }
    public PlayerOxygen Oxygen { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();

        Input = GetComponent<PlayerInputHandler>();
        Movement = GetComponent<PlayerMovement>();
        CombatInput = GetComponent<DungeonCombatInput>();
        Attack = GetComponent<PlayerAttackController>();
        Inventory = GetComponent<PlayerInventory>();
        Equipment = GetComponent<PlayerEquipment>();
        Stats = GetComponent<PlayerStats>();
        Oxygen = GetComponent<PlayerOxygen>();

        var dungeonAnimation = GetComponent<DungeonPlayerAnimation>();
        Movement?.Init(this);
        Attack?.Init(this, dungeonAnimation);

        Stats?.Init(this);
        Oxygen?.Init(this);
        Equipment?.Init(this);
        CombatInput?.Init(this);
    }
}
