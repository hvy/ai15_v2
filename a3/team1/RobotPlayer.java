package team1;

import battlecode.common.*;

import java.util.*;

import team1.Robots.BarracksRobot;
import team1.Robots.BasherRobot;
import team1.Robots.BeaverRobot;
import team1.Robots.HQRobot;
import team1.Robots.MinerFactoryRobot;
import team1.Robots.MinerRobot;
import team1.Robots.SoldierRobot;
import team1.Robots.SupplyDepotRobot;
import team1.Robots.TankFactoryRobot;
import team1.Robots.TankRobot;
import team1.Robots.TowerRobot;

public class RobotPlayer {

	static Robot robot;
	
	public static void run(RobotController rc) {
		//rand = new Random(rc.getID());

		
		
		Direction lastDirection = null;
		
		setRobot(rc);

		while(true) {
			try {
				rc.setIndicatorString(0, "This is an indicator string.");
				rc.setIndicatorString(1, "I am a " + rc.getType());
			} catch (Exception e) {
				System.out.println("Unexpected exception");
				e.printStackTrace();
			}

			
			try {
				robot.update(); // Update robot data
				robot.run(); // Execute AI
			} catch (Exception e) { 
				System.out.println( robot.name() + " Exception");
				e.printStackTrace();
			}
			
			rc.yield();
		}
	}
	
	private static void setRobot(RobotController rc) {
		if (rc.getType() == RobotType.HQ) {
			robot = new HQRobot(rc);
		}

		if (rc.getType() == RobotType.TOWER) {
			robot = new TowerRobot(rc);
		}

		if (rc.getType() == RobotType.BASHER) {
			robot = new BasherRobot(rc);
		}

		if (rc.getType() == RobotType.SOLDIER) {
			robot = new SoldierRobot(rc);
		}

		if (rc.getType() == RobotType.BEAVER) {
			robot = new BeaverRobot(rc);
		}

		if (rc.getType() == RobotType.BARRACKS) {
			robot = new BarracksRobot(rc);
		}
		
		if (rc.getType() == RobotType.MINERFACTORY) {
			robot = new MinerFactoryRobot(rc);
		}
		
		if (rc.getType() == RobotType.MINER) {
			robot = new MinerRobot(rc);
		}
		
		if (rc.getType() == RobotType.SUPPLYDEPOT) {
			robot = new SupplyDepotRobot(rc);
		}
		
		if (rc.getType() == RobotType.TANK) {
			robot = new TankRobot(rc);
		}
		
		if (rc.getType() == RobotType.TANKFACTORY) {
			robot = new TankFactoryRobot(rc);
		}
	}

	
}
