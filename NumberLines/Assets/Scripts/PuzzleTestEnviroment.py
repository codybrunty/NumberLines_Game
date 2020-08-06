#!/usr/bin/env python
from LinePuzzle import Puzzle
from PuzzleWriteOut import WriteOutPuzzle


if __name__ == "__main__":

	testPuzzle = [	"__3___",
				"2__74_",
				"____._",
				"5__..6",
				".4____",
				".._.__",
				"______",
				"__6_3_",
				"2____3"]
	generatePuzzle = ['0']


	lookingForGoodPuzzle=True
	while lookingForGoodPuzzle:
		newPuzzle = Puzzle(generatePuzzle)
		newPuzzle = Puzzle(newPuzzle.GetPuzzleString())
		newPuzzle.SolvePuzzle()
		if newPuzzle.iterations > 5000:
			newPuzzle.ResetBoard()
			WriteOutPuzzle(newPuzzle.GetPuzzleString())
			lookingForGoodPuzzle=False
	#newPuzzle.PrintPuzzle()
	#GameBoard.SolvePuzzle()


