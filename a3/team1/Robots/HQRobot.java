package team1.Robots;


import java.util.HashMap;

import team1.Action;
import team1.Parameters;
import team1.Robot;
import team1.Util;
import battlecode.common.GameActionException;
import battlecode.common.MapLocation;
import battlecode.common.RobotController;
import battlecode.common.RobotInfo;
import battlecode.common.RobotType;

public class HQRobot extends Robot {
	
	double currentSupplyCount = 0f;
	MapLocation armyCheckPoint;
	MapLocation initialCheckpoint;
	HashMap<Integer, RobotInfo> army;
	int armyCount;
		
	
	public HQRobot(RobotController rc) {
		super(rc);
		army = new HashMap<Integer, RobotInfo>();
	}
	
	@Override
	public void update() {
		setArmyCheckPoint();
	}
	
	private void setArmyCheckPoint() {
		if (armyCheckPoint == null) {
			MapLocation[]  towers = rc.senseTowerLocations();
			if (towers.length > 1)
				armyCheckPoint = towers[1];
			else
				armyCheckPoint = home;
			initialCheckpoint = armyCheckPoint;
		}
		
		// if army consists of 10 or more units, start advancing
		RobotInfo[] nearby = rc.senseNearbyRobots(armyCheckPoint, 20, myTeam);
		
		if (nearby.length >= 15) {
			
			army.clear();
			for (int i = 0; i < nearby.length; i++) {
				army.put(nearby[i].ID, nearby[i]);
			}
			
			int x_m = armyCheckPoint.x + (rc.senseEnemyTowerLocations()[0].x - armyCheckPoint.x)/2;
			int y_m = armyCheckPoint.y - (armyCheckPoint.y - rc.senseEnemyTowerLocations()[0].y)/2;
			
			armyCheckPoint = new MapLocation(x_m, y_m);
		} else if (armyCount < 5)
			armyCheckPoint = initialCheckpoint;
		
	}
	
	@Override
	public void run() throws Exception {
//		int fate = rand.nextInt(10000);
		myRobots = rc.senseNearbyRobots(999999, myTeam);
		int numSoldiers = 0;
		int numBashers = 0;
		int numBeavers = 0;
		int numBarracks = 0;
		int numMiners = 0;
		int numMinerFactories = 0;
		int numTankfactories = 0;
		int numUnits = 0;
		int numSupplyDepots = 0;
		
		currentSupplyCount = 0f;
		armyCount = 0;
		for (RobotInfo r : myRobots) {
			currentSupplyCount += r.supplyLevel;
			
			RobotType type = r.type;
			if (type == RobotType.SOLDIER) {
				numSoldiers++;
			} else if (type == RobotType.BASHER) {
				numBashers++;
			} else if (type == RobotType.BEAVER) {
				numBeavers++;
			} else if (type == RobotType.BARRACKS) {
				numBarracks++;
			} else if (type == RobotType.MINER) {
				numMiners++;
			} else if (type == RobotType.MINERFACTORY) {
				numMinerFactories++;
			} else if (type == RobotType.TANKFACTORY) {
				numTankfactories++;
			} else if (type == RobotType.SUPPLYDEPOT) {
				numSupplyDepots++;
			}
			numUnits++;
			
			if (army.containsKey(r.ID))
				armyCount++;
		}
		rc.broadcast(Parameters.BROAD_NUM_BEAVERS, numBeavers);
		rc.broadcast(Parameters.BROAD_NUM_SOLDIERS, numSoldiers);
		rc.broadcast(Parameters.BROAD_NUM_BASHERS, numBashers);
		rc.broadcast(Parameters.BROAD_NUM_MINERS, numMiners);
		rc.broadcast(Parameters.BROAD_NUM_BARRACKS, numBarracks);
		rc.broadcast(Parameters.BROAD_NUM_MIN_FACT, numMinerFactories);
		rc.broadcast(Parameters.BROAD_NUM_TANK_FACT, numTankfactories);
		rc.broadcast(Parameters.BROAD_NUM_SUPPLY_DEPOTS, numSupplyDepots);
		rc.broadcast(Parameters.BROAD_NUM_UNITS, numUnits);
		rc.broadcast(Parameters.BROAD_SUPPLY, (int) currentSupplyCount);
		rc.broadcast(Parameters.BROAD_CHECKPOINT_X, armyCheckPoint.x);
		rc.broadcast(Parameters.BROAD_CHECKPOINT_Y, armyCheckPoint.y);
		
		
		if (rc.isWeaponReady()) {
			Action.attackSomething(myRange, enemyTeam, rc);
		}

		if (rc.isCoreReady() && rc.getTeamOre() >= 100 && numBeavers < Parameters.MAX_BEAVERS) {
			Action.trySpawn(Util.directions[rand.nextInt(8)], RobotType.BEAVER, rc);
		}
		
	}

	@Override
	public String name() {
		return "HQ";
	}





}
