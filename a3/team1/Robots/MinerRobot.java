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
import battlecode.common.RobotType;
import battlecode.common.Team;

public class MinerRobot extends Robot {
	
	private MapLocation mineLoc;
	
	public MinerRobot(RobotController rc) {
		super(rc);
	}
	
	@Override
	public void update() {
		location = rc.getLocation();
        coreReady = rc.isCoreReady();
        weaponReady = rc.isWeaponReady();
	}

	@Override
	public void run() throws Exception {
		if (weaponReady) {
			Action.attackSomething(myRange, enemyTeam, rc);
		}
		
		 SupplyHandler.shareSupply(this);
	     SupplyHandler.requestResupplyIfNecessary(this);
		
		if (rc.isCoreReady()) {			
			doMining();
			coreReady = false;
		}
		
	}
	
	private void doMining() throws GameActionException {
        MapLocation[] enemyTowers = rc.senseEnemyTowerLocations();

//        if (!isSafeToMine(here)) {
//            runAway();
//            mineLoc = null;
//            return;
//        }

        if (mineLoc != null) {
            if (location.equals(mineLoc)) {
                // We are at the spot we want to mine. Decide whether to mine
                // or whether to move on because the spot is exhausted or we are
                // blocking the way
                if (rc.senseOre(location) >= ORE_EXHAUSTED) {
                   
                    rc.mine();
                    return;
                } else {
                    // ore here has been exhausted. choose a new mineLoc
                    mineLoc = null;
//                    Debug.indicate("mine", 0, "ore here exhausted, going to look for a new mine loc");
                }
            } else {
                // we are not at our preferred mineLoc
                if (rc.senseOre(location) >= ORE_EXHAUSTED) {
                    // we happened upon a fine spot to mine that was different from our mineLoc
                    mineLoc = location;
//                    Debug.indicate("mine", 2, "found opportunistic mineLoc here at " + mineLoc.toString() + "; mining; ore left = " + rc.senseOre(here));
                    rc.mine();
                    return;
                } else if (!isValidMineLocation(mineLoc)) {
                    // somehow our mineLoc is no longer suitable :( choose a new one
//                    Debug.indicate("mine", 2, mineLoc.toString() + " is no longer valid :(");
                    mineLoc = null;
                }
            }
        }

        if (mineLoc == null) {
            mineLoc = chooseNewMineLoc();
//            Debug.indicate("mine", 2, "chose new mineLoc: " + mineLoc.toString());
            return; // choosing new mine loc can take several turns
        }

//        Debug.indicate("mine", 1, "going to " + mineLoc.toString());
//        RobotInfo[] nearbyEnemies = rc.senseNearbyRobots(35, them);
//        NavSafetyPolicy safetyPolicy = new SafetyPolicyAvoidAllUnits(enemyTowers, nearbyEnemies);
//        Nav.goTo(mineLoc, safetyPolicy);
          Action.tryMove(location.directionTo(mineLoc), rc);
    }
	
	private int[] legDX = { 0, 0, 0, -1, 0, 0, 0, 1 };
    private int[] legDY = { 0, 1, 0, 0, 0, -1, 0, 0 };
    private double ORE_EXHAUSTED = 3.0f;
	
	 private MapLocation chooseNewMineLoc() {
	        MapLocation searchCenter = location;
	        
	        int maxRadius = 80;

	        Direction startDiag = location.directionTo(home);
	        if (!startDiag.isDiagonal()) startDiag = startDiag.rotateLeft();

	        for (int radius = 1; radius < maxRadius; radius++) {
	            MapLocation bestLoc = null;
	            int bestDistSq = 999999;

	            MapLocation loc = searchCenter.add(startDiag, radius);
	            int diag = startDiag.ordinal();
	            for (int leg = 0; leg < 4; leg++) {
	                int dx = legDX[diag];
	                int dy = legDY[diag];

	                for (int i = 0; i < 2 * radius; i++) {
	                    if (isValidMineLocation(loc)) {
	                        int distSq = home.distanceSquaredTo(loc);
	                        if (distSq < bestDistSq) {
	                            bestDistSq = distSq;
	                            bestLoc = loc;
	                        }
	                    }

	                    loc = loc.add(dx, dy);
	                }

	                diag = (diag + 2) % 8;
	            }

	            if (bestLoc != null) {
	                return bestLoc;
	            }
	        }
	        
	        // we searched a really large region without finding any good ore spots. Lower our standards and recurse
	        ORE_EXHAUSTED = 0.5f;
	        return chooseNewMineLoc();
	    }
	 
	 private boolean isValidMineLocation(MapLocation loc)  {
	        if (rc.senseOre(loc) <= ORE_EXHAUSTED) {
	            return false;
	        }

	        if (locIsOccupied(loc)) return false;
	        
	        if (!isSafeToMine(loc)) return false;

	        return true;
	 }
	 
	 private  boolean locIsOccupied(MapLocation loc) {
		 return (rc.senseNearbyRobots(loc, 1, null).length > 0);
	 }
	 
	 private boolean isSafeToMine(MapLocation loc) {
		 return !(inEnemyTowerOrHQRange(loc, rc.senseEnemyTowerLocations()));
	 }


	@Override
	public String name() {
		return "Miner";
	}



}
