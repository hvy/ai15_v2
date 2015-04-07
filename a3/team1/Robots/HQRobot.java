package team1.Robots;


import java.util.HashMap;

import team1.Action;
import team1.Parameters;
import team1.Robot;
import team1.SupplyHandler;
import team1.Util;
import battlecode.common.GameActionException;
import battlecode.common.GameConstants;
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
	int numSoldiers;
	int numBashers;
	int numBeavers;
	int numBarracks;
	int numMiners;
	int numMinerFactories;
	int numTankfactories;
	int numUnits;
	int numSupplyDepots;
	
	
	public static double totalSupplyGenerated;
		
	
	public HQRobot(RobotController rc) {
		super(rc);
		army = new HashMap<Integer, RobotInfo>();
	}
	
	@Override
	public void update() {
		setArmyCheckPoint();
		totalSupplyGenerated = GameConstants.SUPPLY_GEN_BASE
                * (GameConstants.SUPPLY_GEN_MULTIPLIER + Math.pow(numSupplyDepots, GameConstants.SUPPLY_GEN_EXPONENT));
	}
	
	private void setArmyCheckPoint() {
		if (armyCheckPoint == null) {
			MapLocation[]  towers = rc.senseTowerLocations();
			if (towers.length > 1)
				armyCheckPoint = towers[0];
			else
				armyCheckPoint = home;
			initialCheckpoint = armyCheckPoint;
		}
		
		
		// if army consists of 10 or more units, start advancing
		RobotInfo[] nearby = rc.senseNearbyRobots(armyCheckPoint, 30, myTeam);
		
		if (nearby.length >= 20) {
			
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
		
		numSoldiers = 0;
		numBashers = 0;
		numBeavers = 0;
		numBarracks = 0;
		numMiners = 0;
		numMinerFactories = 0;
		numTankfactories = 0;
		numUnits = 0;
		numSupplyDepots = 0;
		
		
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
		
		broadcast.sendInt(Parameters.BROAD_NUM_BEAVERS, numBeavers);
		broadcast.sendInt(Parameters.BROAD_NUM_SOLDIERS, numSoldiers);
		broadcast.sendInt(Parameters.BROAD_NUM_BASHERS, numBashers);
		broadcast.sendInt(Parameters.BROAD_NUM_MINERS, numMiners);
		broadcast.sendInt(Parameters.BROAD_NUM_BARRACKS, numBarracks);
		broadcast.sendInt(Parameters.BROAD_NUM_MIN_FACT, numMinerFactories);
		broadcast.sendInt(Parameters.BROAD_NUM_TANK_FACT, numTankfactories);
		broadcast.sendInt(Parameters.BROAD_NUM_SUPPLY_DEPOTS, numSupplyDepots);
		broadcast.sendInt(Parameters.BROAD_NUM_UNITS, numUnits);
		broadcast.sendInt(Parameters.BROAD_SUPPLY, (int) currentSupplyCount);
		broadcast.sendLocation(Parameters.BROAD_CHECKPOINT, armyCheckPoint);
		//broadcast.sendInt(Parameters.BROAD_CHECKPOINT_Y, armyCheckPoint.y);
		
		// give supply
		SupplyHandler.hqGiveSupply(this);
		
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
