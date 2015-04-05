package team1.Robots;

import java.util.Random;

import team1.Action;
import team1.Parameters;
import team1.Robot;
import team1.Util;
import battlecode.common.MapLocation;
import battlecode.common.RobotController;
import battlecode.common.RobotInfo;
import battlecode.common.RobotType;
import battlecode.common.Team;

public class MinerFactoryRobot extends Robot {
	
	public MinerFactoryRobot(RobotController rc) {
		super(rc);
	}

	@Override
	public void run() throws Exception {
		int fate = rand.nextInt(10000);

		// get information broadcasted by the HQ
		int numBeavers = rc.readBroadcast(0);
		int numSoldiers = rc.readBroadcast(1);
		int numBashers = rc.readBroadcast(2);
		

		if (rc.isCoreReady() && fate < Math.pow(1.2,15-numSoldiers-numBashers+numBeavers)*10000) {
			if (rc.getTeamOre() > 60 && Parameters.MAX_MINERS > rc.readBroadcast(Parameters.BROAD_NUM_MINERS)) {
				Action.trySpawn(Util.directions[rand.nextInt(8)],RobotType.MINER, rc);
			}
		}
		
	}

	@Override
	public String name() {
		return "MinerFactory";
	}

	@Override
	public void update() {
		coreReady = rc.isCoreReady();
		
	}

}
