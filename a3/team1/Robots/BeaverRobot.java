package team1.Robots;

import java.util.Random;

import team1.Action;
import team1.Parameters;
import team1.Robot;
import team1.SupplyHandler;
import team1.Util;
import battlecode.common.GameActionException;
import battlecode.common.RobotController;
import battlecode.common.RobotInfo;
import battlecode.common.RobotType;
import battlecode.common.Team;

public class BeaverRobot extends Robot {
	
	public BeaverRobot(RobotController rc) {
		super(rc);
	}
	
	@Override
	public void update() {
		location = rc.getLocation();
        coreReady = rc.isCoreReady();
        weaponReady = rc.isWeaponReady();
	}
	
	private boolean needBarrack(int fate) throws GameActionException {
		int numBarracks =  rc.readBroadcast(Parameters.BROAD_NUM_BARRACKS);
		int numMinerFactories = rc.readBroadcast(Parameters.BROAD_NUM_MIN_FACT);
		
		return (fate < 200  || numBarracks == 0) && 
				rc.getTeamOre() >= 300 &&  numMinerFactories != 0 &&
				Parameters.MAX_BARRACKS > numBarracks;
	}
	
	private boolean needMinerFactory(double fate) throws GameActionException {
		int numMinerFactories = rc.readBroadcast(Parameters.BROAD_NUM_MIN_FACT);
		
		return (fate < 300 ) && rc.getTeamOre() >= 500 && numMinerFactories < Parameters.MAX_MINER_FACTORIES;
	}
	
	private boolean needTankFactory(double fate) throws GameActionException {
		int numTankFactories = rc.readBroadcast(Parameters.BROAD_NUM_TANK_FACT);
		int numBarracks =  rc.readBroadcast(Parameters.BROAD_NUM_BARRACKS);
		
		return (fate < 500 ) && rc.getTeamOre() >= 500 && numTankFactories < Parameters.MAX_TANK_FACTORIES && numBarracks > 0;
	}
	
	private boolean needSupplyDepot(double fate) throws GameActionException {
		int numMinerFactories = rc.readBroadcast(Parameters.BROAD_NUM_MIN_FACT);
		int supply = rc.readBroadcast(Parameters.BROAD_SUPPLY);
		int depots = rc.readBroadcast(Parameters.BROAD_NUM_SUPPLY_DEPOTS);
		
		return (fate < 500 && fate > 485) && rc.getTeamOre() >= 100 &&  numMinerFactories != 0;
	}
	
	private boolean shouldMine(double fate) throws GameActionException {
		return fate < 700 || rc.senseOre(location) > Parameters.BEAVER_MINE_THRESHOLD;
	}

	@Override
	public void run() throws Exception {
		if (weaponReady) {
			Action.attackSomething(myRange, enemyTeam, rc);
		}
		
//		SupplyHandler.shareSupply(this);
//	    SupplyHandler.requestResupplyIfNecessary(this);
		
		
		if (rc.isCoreReady()) {
			int fate = rand.nextInt(1000);
			if (needBarrack(fate)) {
				Action.tryBuild(Util.directions[rand.nextInt(8)],RobotType.BARRACKS, rc);
			} else if (needMinerFactory(fate)) {
				Action.tryBuild(Util.directions[rand.nextInt(8)],RobotType.MINERFACTORY, rc);
			} else if (needTankFactory(fate)) {
				Action.tryBuild(Util.directions[rand.nextInt(8)],RobotType.TANKFACTORY, rc);
			} else if (needSupplyDepot(fate)) {
				Action.tryBuild(Util.directions[rand.nextInt(8)],RobotType.SUPPLYDEPOT, rc);
			} else if (shouldMine(fate)) {
				rc.mine();
				coreReady = false;
			} else if (fate < 900) {
				Action.tryMove(Util.directions[rand.nextInt(8)], rc);
			} else {
				Action.tryMove(rc.senseHQLocation().directionTo(rc.getLocation()), rc);
			}
		}
		
	}

	@Override
	public String name() {
		return "Beaver";
	}



}
