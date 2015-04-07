package team1;

import java.util.Random;

import battlecode.common.GameActionException;
import battlecode.common.MapLocation;
import battlecode.common.RobotController;
import battlecode.common.RobotInfo;
import battlecode.common.RobotType;
import battlecode.common.Team;


public abstract class Robot {
	
	protected RobotController rc;
	protected Random rand;
	protected RobotInfo[] myRobots;
	protected Team myTeam;
	protected Team enemyTeam;
	protected int myRange;
	protected MapLocation home;
	protected MapLocation enemyHome;
	
	protected MapLocation location;
	protected boolean coreReady;
	protected  boolean weaponReady;
	
	protected Broadcasting broadcast;
	
	
	public Robot(RobotController rc) {
		this.rc = rc;
		rand = new Random(rc.getID());
		myRange = rc.getType().attackRadiusSquared;
		myTeam = rc.getTeam();
		enemyTeam = myTeam.opponent();
		myRobots = null;
		home = rc.senseHQLocation();
		enemyHome = rc.senseEnemyHQLocation();
		location = rc.getLocation();
		broadcast = new Broadcasting(rc);
	}
	
	public abstract void run() throws Exception;
	
	public abstract void update();
	
	public abstract String name();
	
	protected MapLocation getCheckpoint() throws GameActionException {
		
		return broadcast.readLocation(Parameters.BROAD_CHECKPOINT);
	}
	
	
	protected boolean inEnemyTowerOrHQRange(MapLocation loc, MapLocation[] enemyTowers) {
        if (loc.distanceSquaredTo(enemyHome) <= 52) {
            switch (enemyTowers.length) {
                case 6:
                case 5:
                    // enemy HQ has range of 35 and splash
                    if (loc.add(loc.directionTo(enemyHome)).distanceSquaredTo(enemyHome) <= 35) return true;
                    break;

                case 4:
                case 3:
                case 2:
                    // enemy HQ has range of 35 and no splash
                    if (loc.distanceSquaredTo(enemyHome) <= 35) return true;
                    break;

                case 1:
                case 0:
                default:
                    // enemyHQ has range of 24;
                    if (loc.distanceSquaredTo(enemyHome) <= 24) return true;
                    break;
            }
        }

        for (MapLocation tower : enemyTowers) {
            if (loc.distanceSquaredTo(tower) <= RobotType.TOWER.attackRadiusSquared) return true;
        }

        return false;
    }

}
