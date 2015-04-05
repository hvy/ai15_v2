package team1.Robots;

import java.util.Random;

import team1.Action;
import team1.Robot;
import battlecode.common.RobotController;
import battlecode.common.RobotInfo;
import battlecode.common.Team;

public class TowerRobot extends Robot {

	
	public TowerRobot(RobotController rc) {
		super(rc);
	}

	@Override
	public void run() throws Exception {
		if (rc.isWeaponReady()) {
			Action.attackSomething(myRange, enemyTeam, rc);
		}
		
	}

	@Override
	public String name() {
		return "Tower";
	}

	@Override
	public void update() {
		// TODO Auto-generated method stub
		
	}

}
