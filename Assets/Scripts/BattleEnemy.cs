using UnityEngine;

public class BattleEnemy : MonoBehaviour
{
    public int maxHP = 50;
    public int currentHP;

    private Vector3 originalPosition;

    private void Start()
    {
        currentHP = maxHP;
        originalPosition = transform.position;
    }

    public void MoveForward(Vector3 offset)
    {
        transform.position += offset;
    }

    public void MoveBack()
    {
        transform.position = originalPosition;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log("Inimigo derrotado!");
        }
    }
}