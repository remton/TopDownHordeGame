using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int payPerKill;

    [SerializeField] private int bank;
    private int totalMoneyEarned;
    private int totalKills;

    public delegate void BankChange(int bank);
    public event BankChange EventBankChange;

    private void Start() {
        if (EventBankChange != null) { EventBankChange.Invoke(bank); }
    }

    public void AddMoney(int amount) {
        bank += amount;
        totalMoneyEarned += amount;
        if (EventBankChange != null) { EventBankChange.Invoke(bank); }
    }
    public bool SpendMoney(int amount) {
        if (bank >= amount) {
            bank -= amount;
            if (EventBankChange != null) { EventBankChange.Invoke(bank); }
            return true;
        }
        return false;
    }

    public void AddKill() {
        totalKills++;
        AddMoney(payPerKill);
    }


}
