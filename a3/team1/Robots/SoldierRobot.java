package team1.Robots;

import java.util.Random;

import team1.Action;
import team1.Parameters;
import team1.Robot;
import team1.SupplyHandler;
import team1.Util;
import battlecode.common.Direction;
import battlecode.common.GameActionException;
import battlecode.common.MapLocation;
import battlecode.common.RobotController;
import battlecode.common.RobotInfo;
import battlecode.common.Team;

public class SoldierRobot extends Robot {

	
	public SoldierRobot(RobotController rc) {
		super(rc);
	}

	@Override
	public void run() throws Exception {
		if (rc.isWeaponReady()) {
			Action.attackSomething(myRange, enemyTeam, rc);
//			return;
		}
		
		SupplyHandler.shareSupply(this);
	    SupplyHandler.requestResupplyIfNecessary(this);
	     
		if (rc.isCoreReady()) {
			int fate = rand.nextInt(1000);
			if (fate < 30) {
				Action.tryMove(Util.directions[rand.nextInt(8)], rc);
			} else {
				
				//Action.tryMove(rc.getLocation().directionTo(rc.senseEnemyHQLocation()), rc);
				if (rc.senseTowerLocations().length == 0)
					Action.tryMove(rc.getLocation().directionTo(home), rc);
				else 
					Action.tryMove(rc.getLocation().directionTo(getCheckpoint()), rc);
			}
		}
		
	}
	

	@Override
	public String name() {
		return "Soldier";
	}

	@Override
	public void update() {
		// TODO Auto-generated method stub
		
	}

}
