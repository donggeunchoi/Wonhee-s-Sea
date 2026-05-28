using UnityEditor;
using UnityEngine;

public class EquipmentDebug : EditorWindow
{
    private PlayerRoot _player;
    private Vector2 _scroll;

    [MenuItem("Tools/Sea/Equipment Debug Window")]
    public static void Open()
    {
        GetWindow<EquipmentDebug>("Equipment Debug");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Equipment Debug Tool", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("플레이 모드에서 플레이어의 무기/장비를 테스트하는 툴입니다.", MessageType.Info);

        EditorGUILayout.Space(4);

        _player = (PlayerRoot)EditorGUILayout.ObjectField(
            "Player",
            _player,
            typeof(PlayerRoot),
            true
        );

        if (_player == null)
        {
            EditorGUILayout.HelpBox("PlayerRoot가 붙은 플레이어를 넣어주세요.", MessageType.Warning);

            if (GUILayout.Button("현재 씬에서 PlayerRoot 자동 찾기"))
            {
                _player = FindObjectOfType<PlayerRoot>();
            }

            return;
        }

        var equip = _player.Equipment;
        var stats = _player.Stats;

        if (equip == null)
        {
            EditorGUILayout.HelpBox("선택한 오브젝트에 PlayerEquipment가 없습니다.", MessageType.Error);
            return;
        }

        using (new EditorGUI.DisabledScope(!Application.isPlaying))
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("무기", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("단검 장착", GUILayout.Height(30)))
            {
                equip.EquipDagger();
                RefreshEditor();
            }

            if (GUILayout.Button("작살총 장착", GUILayout.Height(30)))
            {
                equip.EquipHarpoon();
                RefreshEditor();
            }

            if (GUILayout.Button("따발총 장착", GUILayout.Height(30)))
            {
                equip.EquipSMG();
                RefreshEditor();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("장비", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("모자 장착", GUILayout.Height(28)))
            {
                equip.EquipHat();
                RefreshEditor();
            }

            if (GUILayout.Button("모자 해제", GUILayout.Height(28)))
            {
                equip.UnequipHat();
                RefreshEditor();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("갑옷 장착", GUILayout.Height(28)))
            {
                equip.EquipArmor();
                RefreshEditor();
            }

            if (GUILayout.Button("갑옷 해제", GUILayout.Height(28)))
            {
                equip.UnequipArmor();
                RefreshEditor();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("신발 장착", GUILayout.Height(28)))
            {
                equip.EquipShoes();
                RefreshEditor();
            }

            if (GUILayout.Button("신발 해제", GUILayout.Height(28)))
            {
                equip.UnequipShoes();
                RefreshEditor();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            if (GUILayout.Button("장비 전체 해제", GUILayout.Height(30)))
            {
                equip.UnequipHat();
                equip.UnequipArmor();
                equip.UnequipShoes();
                RefreshEditor();
            }

            EditorGUILayout.Space(14);
            EditorGUILayout.LabelField("현재 상태", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("현재 무기", equip.CurrentWeapon.ToString());
            EditorGUILayout.LabelField("모자", equip.HasHat ? "장착" : "없음");
            EditorGUILayout.LabelField("갑옷", equip.HasArmor ? "장착" : "없음");
            EditorGUILayout.LabelField("신발", equip.HasShoes ? "장착" : "없음");

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("보너스 수치", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Bonus Hp", equip.BonusHp.ToString());
            EditorGUILayout.LabelField("Bonus Move Speed", equip.BonusMoveSpeed.ToString());
            EditorGUILayout.LabelField("Bonus Oxygen", equip.BonusOxygen.ToString());

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("플레이어 상태", EditorStyles.boldLabel);

            if (stats != null)
            {
                EditorGUILayout.LabelField("Current Hp", stats.CurrentHp.ToString());
                EditorGUILayout.LabelField("Max Hp", stats.MaxHp.ToString());
            }
            else
            {
                EditorGUILayout.LabelField("PlayerStats 없음");
            }

            EditorGUILayout.EndScrollView();
        }

        if (!Application.isPlaying)
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("플레이 모드에서만 장착 버튼을 사용할 수 있습니다.", MessageType.None);
        }
    }

    private void RefreshEditor()
    {
        Repaint();
        SceneView.RepaintAll();

        if (_player != null)
            EditorUtility.SetDirty(_player);
    }
}
