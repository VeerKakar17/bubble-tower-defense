using System;


public static class Rounds
{
    public class EnemyInfo {
        public int dmg;
        public float moveSpeed;
        public int health;
        public int moneyWorth;
        public int spawn;

        // Enemy Stats
        public EnemyInfo(int dmg, float moveSpeed, int health, int moneyWorth, int spawn)
        {
            this.dmg = dmg;
            this.moveSpeed = moveSpeed;
            this.health = health;
            this.moneyWorth = moneyWorth;
            this.spawn = spawn;
        }
    }
    /// <summary>
    /// Formatting is enemyidxNumberOfTimes/spacingTimeMilliseconds/modifiers, split by spaces, add W+seconds for wait x milliseconds.
    /// Example: 0x10/100/ W500 1x3/500/C
    /// </summary>
    public static String[] roundInfo = {
        "0x1/1/",
        "0x30/100/ W2000 2x10/500/ W5000 1x10/200/ W1000 3x5/400/ W1000 4x3/77/",
        "4x20/10/C"
    };


    public static EnemyInfo[] enemies = {
        new EnemyInfo(1, 2f, 1, 1, -1),
        new EnemyInfo(2, 4f, 1, 1, 0),
        new EnemyInfo(4, 6f, 1, 1, 1),
        new EnemyInfo(6, 2f, 5, 2, 2),
        new EnemyInfo(8, 12f, 1, 3, 3)
    };
}
