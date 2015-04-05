package team1;

import battlecode.common.Direction;
import battlecode.common.GameActionException;
import battlecode.common.RobotController;
import battlecode.common.RobotInfo;
import battlecode.common.RobotType;
import battlecode.common.Team;

public class Action {
	
	// This method will attack an enemy in sight, if there is one
	public static void attackSomething(int myRange, Team enemyTeam, RobotController rc) throws GameActionException {
		RobotInfo[] enemies = rc.senseNearbyRobots(myRange, enemyTeam);
		if (enemies.length > 0) {
			rc.attackLocation(enemies[0].location);
		}
	}
	
	// This method will attempt to move in Direction d (or as close to it as possible)
	public static void tryMove(Direction d, RobotController rc) throws GameActionException {
		int offsetIndex = 0;
		int[] offsets = {0,1,-1,2,-2};
		int dirint = Util.directionToInt(d);
		boolean blocked = false;
		while (offsetIndex < 5 && !rc.canMove(Util.directions[(dirint+offsets[offsetIndex]+8)%8])) {
			offsetIndex++;
		}
		if (offsetIndex < 5) {
			rc.move(Util.directions[(dirint+offsets[offsetIndex]+8)%8]);
		}
	}

	// This method will attempt to spawn in the given direction (or as close to it as possible)
	public static void trySpawn(Direction d, RobotType type, RobotController rc) throws GameActionException {
		int offsetIndex = 0;
		int[] offsets = {0,1,-1,2,-2,3,-3,4};
		int dirint = Util.directionToInt(d);
		boolean blocked = false;
		while (offsetIndex < 8 && !rc.canSpawn(Util.directions[(dirint+offsets[offsetIndex]+8)%8], type)) {
			offsetIndex++;
		}
		if (offsetIndex < 8) {
			rc.spawn(Util.directions[(dirint+offsets[offsetIndex]+8)%8], type);
		}
	}

	// This method will attempt to build in the given direction (or as close to it as possible)
	public static void tryBuild(Direction d, RobotType type, RobotController rc) throws GameActionException {
		int offsetIndex = 0;
		int[] offsets = {0,1,-1,2,-2,3,-3,4};
		int dirint = Util.directionToInt(d);
		boolean blocked = false;
		while (offsetIndex < 8 && !rc.canMove(Util.directions[(dirint+offsets[offsetIndex]+8)%8])) {
			offsetIndex++;
		}
		if (offsetIndex < 8) {
			rc.build(Util.directions[(dirint+offsets[offsetIndex]+8)%8], type);
		}
	}

}
