using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamInventory
{
    public List<EquipmentAmount> items = new List<EquipmentAmount>();

    public void SetItems(List<EquipmentAmount> startItems)
    {
        foreach (EquipmentAmount amt in startItems)
            items.Add(new EquipmentAmount(amt));
    }

    public WeaponData TryGetGun() => TryGetItemOfTypeForBot(WeaponData.WeaponType.Gun);

    public WeaponData TryGetProjectile() => TryGetItemOfTypeForBot(WeaponData.WeaponType.Projectile);

    public WeaponData TryGetExplosive() => TryGetItemOfTypeForBot(WeaponData.WeaponType.Explosive);

    private WeaponData TryGetItemOfTypeForBot(WeaponData.WeaponType wtype)
    {
        List<WeaponData> potentials = new List<WeaponData>();

        foreach (var itm in items)
        {
            if (itm.data.canBotUse && itm.amount > 0 && itm.data.weaponType == wtype)
                potentials.Add(itm.data);
        }

        if (potentials.Count == 0)
           return null;

        return potentials[Random.Range(0, potentials.Count)];
    }

    public List<WeaponData> GetAvailableBotWeapons()
    {
        List<WeaponData> outList = new List<WeaponData>();
        
        foreach (var itm in items)
        {
            if (itm.data.canBotUse && itm.amount > 0)
                outList.Add(itm.data);
        }

        return outList;
    }

    public int GetAmount(WeaponData wep)
    {
        foreach (var itm in items)
        {
            if (itm.data == wep)
                return itm.amount;
        }

        return 0;
    }

    public void UseItem(WeaponData wep)
    {
        foreach (var itm in items)
        {
            if (itm.data == wep)
            {
                itm.amount --;
                return;
            }
        }
    }
}

public class TeamManager : MonoBehaviour
{
    public List<Player> allPlayers;

    private Dictionary<TeamData, List<Player>> allTeams;

    public List<TeamData> AllTeams => new List<TeamData>(allTeams.Keys);

    [SerializeField] private GameSettings _settings;

    LevelData _level;

    public Player GetRandomMemberOfTeam(TeamData team)
    {
        List<Player> potentials = new List<Player>(allTeams[team].Where(p => p.PlayerHealth > 0));
        return potentials[Random.Range(0, potentials.Count)];
    }

    // Start is called before the first frame update
    public void Init()
    {
        DiscoverAllPlayers();
        InitialisePlayerNumbers();
    }


    void DiscoverAllPlayers()
    {
        allPlayers = new List<Player>();
        allTeams = new Dictionary<TeamData, List<Player>>();

        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.Team == null)
                continue;

            if (allTeams.ContainsKey(player.Team) == false)
                allTeams[player.Team] = new List<Player>();

            allPlayers.Add(player);
            allTeams[player.Team].Add(player);
        }
        CalculateTotalTeamHealth();      
    }

    void InitialisePlayerNumbers()
    {
        foreach (TeamData team in allTeams.Keys)
        {
            TeamInventory inv = new TeamInventory();
            inv.SetItems(_settings.GetCurrentLevel().startLoadout);

            for (int i=0; i<allTeams[team].Count; i++)
            {
                allTeams[team][i].SetPlayerID(i);
                allTeams[team][i].Equipment.inventory = inv;
                allTeams[team][i].SpawnPlayerUI();
            }
        }
    }

    public TeamHealthInfo CalculateTotalTeamHealth()
    {
        TeamHealthInfo info = new TeamHealthInfo();
        
        foreach(TeamData team in allTeams.Keys)
        {
            int thisTeamTotal = 0;
            int thisTeamCurrent = 0;
            foreach(Player p in allTeams[team])
            {
                thisTeamCurrent += p.PlayerHealth;
                thisTeamTotal += p.MaxHealth;
            }

            info.teamCurrentHealth[team] = thisTeamCurrent;
            info.teamMaxHealth[team] = thisTeamTotal;
            
            info.maxHealthOfAnyTeam = Mathf.Max(info.maxHealthOfAnyTeam, thisTeamTotal);
        }

        return info;
    }       
}

[System.Serializable]
public class TeamHealthInfo
{
    public int maxHealthOfAnyTeam;
    public Dictionary<TeamData, int> teamCurrentHealth = new Dictionary<TeamData, int>();
    public Dictionary<TeamData, int> teamMaxHealth = new Dictionary<TeamData, int>();

    public List<TeamData> SortedListOfTeams()
    {
        List<TeamData> outList = new List<TeamData>(teamCurrentHealth.Keys);

        return outList.OrderBy(t => -teamCurrentHealth[t]).ToList();
    }
}
