using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerStats : NetworkBehaviour
{
    // ----- SYNCED STATS -----
    #region stats

    // NAME
    [SyncVar] 
    private string playerName = "NULL";
    public string GetName() {
        return playerName;
    }
    private void OnNameChange(string oldName, string newName) {
        playerName = newName;
        if (EventNameChange != null) { EventNameChange.Invoke(playerName); }
    }
    public delegate void NameChange(string newName);
    public event NameChange EventNameChange;
    [Server]
    public void SetName(string newName) {
        playerName = newName;
    }

    // BANK
    [SyncVar(hook = nameof(OnBankChange))] 
    private int bank;
    public int GetBank() { return bank; }
    private void OnBankChange(int oldBank, int newBank) {
        bank = newBank;
        if (EventBankChange != null) { EventBankChange.Invoke(bank); }
    }
    public delegate void BankChange(int bank);
    public event BankChange EventBankChange;
    [Server]
    public void SetBank(int newBank) {
        bank = newBank;
    }

    // SCORE
    [SyncVar(hook = nameof(OnScoreChange))] 
    private int score;
    public int GetScore() { return score; }
    public int GetTotalScore() {
        return score + totalKills / 5 + totalMoneyEarned / 10 + (RoundController.instance.round - 1) * 50;
    }
    private void OnScoreChange(int oldScore, int newScore) {
        score = newScore;
        if (EventScoreChange != null) { EventScoreChange.Invoke(GetTotalScore()); }
    }
    public delegate void ScoreChange(int score);
    public event ScoreChange EventScoreChange;
    [Server]
    public void SetScore(int newScore) {
        score = newScore;
    }

    // TOTAL MONEY
    [SyncVar] private int totalMoneyEarned;
    public int GetTotalMoney() { return totalMoneyEarned; }
    public void OnTotalMoneyChange(int oldTotal, int newTotal) {
        totalMoneyEarned = newTotal;
        if (EventTotalMoneyChange != null) { EventTotalMoneyChange.Invoke(totalMoneyEarned); }
    }
    public delegate void TotalMoneyChange(int newTotal);
    public event TotalMoneyChange EventTotalMoneyChange;
    [Server]
    public void SetTotalMoney(int newTotal) {
        totalMoneyEarned = newTotal;
    }

    // TOTAL KILLS
    [SyncVar] private int totalKills;
    public int GetTotalKills() { return totalKills; }
    private void OnTotalKillsChange(int oldTotal, int newTotal) {
        if (EventTotalKillsChange != null) { EventTotalKillsChange.Invoke(totalKills); }
    }
    public delegate void TotalKillsChange(int total);
    public event TotalKillsChange EventTotalKillsChange;
    [Server]
    public void SetTotalKills(int newTotal) {
        totalKills = newTotal;
    }
    #endregion

    // -------- METHODS --------

    [Server]
    public void AddScore(int addScore) { 
        score += addScore;
    }
    [Server]
    public void AddMoney(int amount) {
        bank += amount;
        totalMoneyEarned += amount;
    }
    [Client]
    public bool TrySpendMoney(int amount) {
        if (bank >= amount) {
            SpendMoney(amount);
            return true;
        }
        return false;
    }
    [Command]
    public void SpendMoney(int amount) {
        bank -= amount;
    }

    [Server]
    public void AddKill() {
        totalKills++;
    }

    // ----- Editor buttons -----
    [SerializeField] bool editor_addMoney = false;
    [SerializeField] int editor_amount = 1000;
    private void Update() {
        if (editor_addMoney) {
            editor_addMoney = false;
            AddMoney(editor_amount);
        }
    }

}
