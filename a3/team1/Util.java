package team1;

import battlecode.common.Direction;

public class Util {
	
	public static Direction[] directions = {Direction.NORTH, Direction.NORTH_EAST, Direction.EAST, Direction.SOUTH_EAST, Direction.SOUTH, Direction.SOUTH_WEST, Direction.WEST, Direction.NORTH_WEST};

	public static int directionToInt(Direction d) {
		switch(d) {
			case NORTH:
				return 0;
			case NORTH_EAST:
				return 1;
			case EAST:
				return 2;
			case SOUTH_EAST:
				return 3;
			case SOUTH:
				return 4;
			case SOUTH_WEST:
				return 5;
			case WEST:
				return 6;
			case NORTH_WEST:
				return 7;
			default:
				return -1;
		}
	}
	
	
	public static int distance(int i, int j, int x, int y) {
		return Math.max(Math.abs(i - x), Math.abs(j - y));
	}

	public static float distance(float x, float y) {
		return Math.max(Math.abs(x), Math.abs(y));
	}
	
}
