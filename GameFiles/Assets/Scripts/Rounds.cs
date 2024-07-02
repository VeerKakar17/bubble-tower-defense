using System;
using UnityEngine;


public static class Rounds
{
    public class EnemyInfo {
        public int dmg;
        public float moveSpeed;
        public int health;
        public int moneyWorth;
        public int[] spawn;
        public bool scales;
        public float size;

        /// <summary>
        /// Declare Enemy Stats
        /// </summary>
        /// <param name="dmg">how much health it deals when reaching the exit</param>
        /// <param name="moveSpeed">how fast it moves</param>
        /// <param name="health">how many hits it takes before dying or summoning spawns</param>
        /// <param name="moneyWorth">how much money is gained upon killing it. If scales, then it's how much money per layer.</param>
        /// <param name="size">scale factor of the bubble. If 1, then normal size</param>
        /// <param name="scales">true if it becomes smaller and slower the less hp it has, false otherwise (defalt is true)</param>
        /// <param name="spawn">Array of id's of what enemies this spawns. default is null</param>
        public EnemyInfo(int dmg, float moveSpeed, int health, int moneyWorth, float size, bool scales = true, int[] spawn = null)
        {
            this.dmg = dmg;
            this.moveSpeed = moveSpeed;
            this.health = health;
            this.moneyWorth = moneyWorth;
            this.spawn = spawn;
            this.scales = scales;
            this.size = size;
        }
    }
    /// <summary>
    /// Formatting is enemyidxNumberOfTimes/spacingTimeMilliseconds/modifiers, split by spaces, add W+seconds for wait x milliseconds.
    /// Example: 0x10/100/ W500 1x3/500/C
    /// </summary>
    public static String[] roundInfo = {
        "10x3/1000/",
        "1x10/100/",
        "9x5/100/ W5000 10x3/200/",
        ""
    };

    private const float SPEED_SCALE = 1f;
    private const float SIZE_SCALE = 0.5f;
    private const float START_SIZE = 3f;
    private const float START_SPEED = 1f;



    public static EnemyInfo[] enemies = {
        new EnemyInfo(1, START_SPEED, 1, 1, START_SIZE),                                                                    //0
        new EnemyInfo(2, START_SPEED+SPEED_SCALE, 2, 1, START_SIZE+SIZE_SCALE),                                             //1
        new EnemyInfo(3, START_SPEED+SPEED_SCALE*2, 3, 1, START_SIZE+SIZE_SCALE*2),                                         //2
        new EnemyInfo(4, START_SPEED+SPEED_SCALE*3, 3, 1, START_SIZE+SIZE_SCALE*3),                                         //3
        new EnemyInfo(5, START_SPEED+SPEED_SCALE*4, 4, 1, START_SIZE+SIZE_SCALE*4),                                         //4
        new EnemyInfo(6, START_SPEED+SPEED_SCALE*5, 5, 1, START_SIZE+SIZE_SCALE*5),                                         //5
        new EnemyInfo(7, START_SPEED+SPEED_SCALE*6, 6, 1, START_SIZE+SIZE_SCALE*6),                                         //6
        new EnemyInfo(8, START_SPEED+SPEED_SCALE*7, 7, 1, START_SIZE+SIZE_SCALE*7),                                         //7
        new EnemyInfo(9, START_SPEED+SPEED_SCALE*8, 8, 1, START_SIZE+SIZE_SCALE*8),                                         //8
        new EnemyInfo(10, START_SPEED+SPEED_SCALE*9, 9, 1, START_SIZE+SIZE_SCALE*9),                                        //9
        new EnemyInfo(20, START_SPEED+SPEED_SCALE*4, 10, 10, START_SIZE+SIZE_SCALE*9, false, new int[]{9, 9, 9}),           //10
        new EnemyInfo(50, START_SPEED+SPEED_SCALE*3, 100, 0, START_SIZE+SIZE_SCALE*12, false, new int[]{10, 10, 10, 10}),   //11
    };
}
