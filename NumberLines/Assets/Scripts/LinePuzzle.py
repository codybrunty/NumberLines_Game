from PuzzleSquare import Square
import random

class Puzzle():
	AllSquares=[]
	AllNumbers=[]
	width=0
	height=0

	def __init__(self,puzzleString):
		if puzzleString == ['0']:
			#print("no puzzle so creating one")
			self.GenrateNewPuzzle()
		else:
			self.puzzleString = puzzleString
			self.height = len(puzzleString)
			self.width = len(puzzleString[0])
			self.BuildPuzzle(self.puzzleString)
			self.SetSquareMoves()

	def GenrateNewPuzzle(self):
		self.width = 6
		self.height = 9
		self.minNumber = 2
		self.maxNumber = 7
		self.minDotSquares = 2
		self.maxDotsSquares = 7
		self.minEmptySquares = 8
		self.maxEmptySquares = 26

		self.GenerateNewPuzzleString()
		self.BuildPuzzle(self.puzzleString)
		self.AddNumbersToPuzzle()
		self.PrintPuzzle()
		self.AddDotsToPuzzle()
		print("\n\n\n")
		self.PrintPuzzle()

	def GetPuzzleString(self):
		puzzleString = []
		line=""
		for i in range(len(self.AllSquares)):
			if i % self.width == 0 and i!=0:
				puzzleString.append(line)
				line=""


			line+=self.AllSquares[i].v

		puzzleString.append(line)
		return puzzleString

	def AddDotsToPuzzle(self):
		lineSquareIndexes = []
		for square in self.AllSquares:
			if square.v == '-' or square.v == '|':
				lineSquareIndexes.append(square.index)

		randomAmmountOfDots = random.randrange(self.minDotSquares,self.maxDotsSquares+1)

		while randomAmmountOfDots > 0:
			randomlineSquareIndex = random.randrange(0,len(lineSquareIndexes))
			if self.AllSquares[randomlineSquareIndex].v == '-' or self.AllSquares[randomlineSquareIndex].v == '|':
				self.AllSquares[randomlineSquareIndex].SetV('.')
				randomAmmountOfDots-=1

		for square in self.AllSquares:
			if square.v == '-' or square.v == '|':
				square.SetV('_')


	def AddNumbersToPuzzle(self):
		emptySquaresThreshold = random.randrange(self.minEmptySquares,self.maxEmptySquares+1)
		emptySquareCount = self.GetAmmountOfEmptySquares()

		tryAddingCounter=0
		while emptySquareCount > emptySquaresThreshold and tryAddingCounter<1000:
			number = random.randrange(self.minNumber,self.maxNumber+1)
			self.AddNumberToBoard(number)
			emptySquareCount = self.GetAmmountOfEmptySquares()
			tryAddingCounter+=1
			#print (emptySquareCount)
		
		if tryAddingCounter>10000:
			print("went over tries")
			print (emptySquareCount)
			print (emptySquaresThreshold)

	def AddNumberToBoard(self,number):
		
		addingNumber = True

		tryAddingCounter=0
		while addingNumber and tryAddingCounter<10000:
			blankSquareIndexes = self.GetAllBlankSquareIndexes()
			randomIndex = blankSquareIndexes[random.randrange(0,len(blankSquareIndexes))]

			if self.AllSquares[randomIndex].v == '_':


				self.AllSquares[randomIndex].SetV(str(number))
				self.SetAllVertMoves(self.AllSquares[randomIndex])
				self.SetAllHorzMoves(self.AllSquares[randomIndex])


				if len(self.AllSquares[randomIndex].moves) > 0:

					#self.DeleteGeneratedBadMoves(self.AllSquares[randomIndex])

					randomMovesIndex = random.randrange(0,len(self.AllSquares[randomIndex].moves))
					self.AddNumberAndMoveToBoard(self.AllSquares[randomIndex],randomMovesIndex)
					addingNumber=False
				else:
					self.AllSquares[randomIndex].SetV('_')
					self.AllSquares[randomIndex].moves=[]
					randomIndex = random.randrange(0,len(self.AllSquares))


			else:
				randomIndex = random.randrange(0,len(self.AllSquares))

			tryAddingCounter+=1

	def GetAllBlankSquareIndexes(self):
		blankSquares=[]
		for square in self.AllSquares:
			if square.v == '_':
				blankSquares.append(square.index)

		return blankSquares

	def DeleteGeneratedBadMoves(self,square):
		goodMoves = []
		for i in range(len(square.moves)):
			moveString = square.moves[i]
			if moveString[0] == 'h':
				isGood = self.CheckHorzMoveQuality(int(moveString[1:]),int(square.v),int(square.index))
			else:
				isGood = self.CheckVertMoveQuality(int(moveString[1:]),int(square.v),int(square.index))

			if isGood:
				goodMoves.append(square.moves[i])
		
		print (goodMoves)
		square.moves=goodMoves


	def CheckHorzMoveQuality(self,moveStartIndex,numberSpaces,squareIndex):
		isGood=True
		for i in range(numberSpaces):
			if self.AllSquares[moveStartIndex+i].v != '_' and int(self.AllSquares[moveStartIndex+i].index) != int(squareIndex):
				print("delete")
				isGood=False
		return isGood

	def CheckVertMoveQuality(self,moveStartIndex,numberSpaces,squareIndex):
		isGood=True
		for i in range(numberSpaces):
			if self.AllSquares[moveStartIndex+(i*self.width)].v != '_' and int(self.AllSquares[moveStartIndex+(i*self.width)].index) != int(squareIndex):
				isGood=False
		return isGood


	def AddNumberAndMoveToBoard(self,square,moveIndex):
		moveString = square.moves[moveIndex]

		if moveString[0] == 'h':
			self.AddHorzMove(int(moveString[1:]),int(square.v))
		else:
			self.AddVertMove(int(moveString[1:]),int(square.v))

	def AddHorzMove(self,moveStartIndex,numberSpaces):
		for i in range(numberSpaces):
			if self.AllSquares[moveStartIndex+i].v=='_':
				self.AllSquares[moveStartIndex+i].SetV('-')

	def AddVertMove(self,moveStartIndex,numberSpaces):
		for i in range(numberSpaces):
			if self.AllSquares[moveStartIndex+(i*self.width)].v=='_':
				self.AllSquares[moveStartIndex+(i*self.width)].SetV('|')


	def GetAmmountOfEmptySquares(self):
		emptySquareCount = 0
		for square in self.AllSquares:
			if square.v == '_':
				emptySquareCount+=1
		return emptySquareCount


	def GenerateNewPuzzleString(self):
		self.puzzleString=[]
		for i in range(self.height):
			line="_"*self.width
			self.puzzleString.append(line)


	def BuildPuzzle(self,puzzleString):
		index=0
		numbers = ['1','2','3','4','5','6','7','8','9']
		self.AllSquares=[]
		self.AllNumbers=[]
		for y in range(self.height):
			for x in range(self.width):
				square=Square(index,x,y,puzzleString[y][x])
				self.AllSquares.append(square)
				if puzzleString[y][x] in numbers:
					self.AllNumbers.append(square)
				index+=1

	def SetSquareMoves(self):
		for square in self.AllNumbers:
			square.moves=[]
			self.SetAllVertMoves(square)
			self.SetAllHorzMoves(square)

			print ("Number "+ str(square.v) +" Index " + str(square.index) +" has "+str(len(square.moves))+" moves")
			print(square.moves)

	def SetAllVertMoves(self,square):
		index = square.index
		vertMoveStartingIndex = [str(index)]
		number = int(square.v)

		for i in range(1,number):
			index-=self.width
			vertMoveStartingIndex.append(str(index))

		vertMoveStartingIndex = self.DeleteImpossibleVertMoves(vertMoveStartingIndex,number,square.index)
		square.moves = vertMoveStartingIndex


	def SetAllHorzMoves(self,square):
		index = square.index
		yindex=int(square.x)
		x=int(square.y)
		horzMoveStartingIndex = [str(yindex)]
		number = int(square.v)
		

		for i in range(1,number):
			yindex -= 1
			horzMoveStartingIndex.append(str(yindex))

		horzMoveStartingIndex = self.DeleteImpossibleHorzMoves(horzMoveStartingIndex,number,x,square.index)
		square.moves += horzMoveStartingIndex

	def DeleteImpossibleHorzMoves(self,movelist,number,x,indexCheck):
		#print(movelist)
		possibleHorzMoves=[]
		for move in movelist:
			index=int(move)
			if index > -1:
				moveSquares=[index]
				for i in range(1,number):
					index+=1
					moveSquares.append(index)

				moveSquaresCheck=True
				for eachSquare in moveSquares:
					if eachSquare > -1 and eachSquare < self.width:
						pass
					else:
						moveSquaresCheck=False
				

				if len(moveSquares) > 1 and moveSquaresCheck==True:
					for newi in  range (len(moveSquares)):
						if self.AllSquares[ moveSquares[newi] + x*self.width ].v != '_' and self.AllSquares[ moveSquares[newi] + x*self.width ].v != '.':
							if self.AllSquares[ moveSquares[newi] + x*self.width ].index != indexCheck:
								moveSquaresCheck=False


				if moveSquaresCheck == True:
					possibleHorzMoves.append("h"+ str((int(move) + x*self.width)))

		return possibleHorzMoves

	def DeleteImpossibleVertMoves(self,movelist,number,indexCheck):

		possibleVertMoves=[]
		for move in movelist:
			index=int(move)
			if index > -1:
				moveSquares=[index]
				for i in range(1,number):
					index+=self.width
					moveSquares.append(index)

				moveSquaresCheck=True
				for eachSquare in moveSquares:
					if eachSquare > -1 and eachSquare < ((self.width*self.height)):
						pass
					else:
						moveSquaresCheck=False


				if len(moveSquares) > 1 and moveSquaresCheck==True:
					for newi in  range (len(moveSquares)):
						if self.AllSquares[moveSquares[newi]].v != '_' and self.AllSquares[moveSquares[newi]].v != '.':
							if self.AllSquares[moveSquares[newi]].index != indexCheck:
								moveSquaresCheck=False

				if moveSquaresCheck == True:
					possibleVertMoves.append("v"+move)

		return possibleVertMoves

	def PrintPuzzle(self):
		for square in self.AllSquares:
			if square.index % self.width == 0 and square.index != 0:
				print("\n")
			print(str(square.v),end='')
		print("\n")

	def GetNumberSquares(self):
		AllNumberSquares = []
		for square in self.AllSquares:
			if square.v in numbers:
				AllNumberSquares.append(square)
		return AllNumberSquares

	def GetNumberSquaresVertical(self,dotSquare):
		AllNumberSquaresVertical=[]
		numbers = ['1','2','3','4','5','6','7','8','9']
		for square in self.AllSquares:
			if square.y == dotSquare.y and square != dotSquare and square.v in numbers and square.completed == False:
				AllNumberSquaresVertical.append(square)
		return AllNumberSquaresVertical

	def GetNumberSquaresHorizontal(self,dotSquare):
		AllNumberSquaresHorizontal=[]
		numbers = ['1','2','3','4','5','6','7','8','9']
		for square in self.AllSquares:
			if square.x == dotSquare.x and square != dotSquare and square.v in numbers and square.completed == False:
				AllNumberSquaresHorizontal.append(square)
		return AllNumberSquaresHorizontal
				
	def SolvePuzzle(self):
		print("trying to solve")
		solved=False
		moveIndexes = [1] * len(self.AllNumbers)
		i=0
		while solved == False:
			print(moveIndexes)
			results = self.TryMoves(moveIndexes)
			#print(moveIndexes)
			if results == False:
				moveIndexes = self.IterateMoveIndexes(moveIndexes)
				if moveIndexes == [0]*len(moveIndexes):
					solved=True
				
			else:
				periodResults = self.CheckForPeriods()
				if periodResults == True:
					moveIndexes = self.IterateMoveIndexes(moveIndexes)
					if moveIndexes == [0]*len(moveIndexes):
						solved=True
					else:
						self.ResetBoard()
				else:
					solved=True
			
			i+=1
				

		self.iterations = i
		print("iteration number "+str(i))	
		self.PrintPuzzle()

	def CheckForPeriods(self):
		periodResults = False
		for square in self.AllSquares:
			if square.v == '.':
				periodResults=True
				break
		return periodResults

	def TryMoves(self,moveIndexes):
		results = True
		for i in range(len(self.AllNumbers)):
			number = self.AllNumbers[i].v
			move = self.AllNumbers[i].moves[moveIndexes[i]-1]
			direction = move[0]
			if direction == 'h':
				results = self.TryHorzMove(int(move[1:]),number)
			else:
				results = self.TryVertMove(int(move[1:]),number)

			if results == False:
				break	

		if results == True:
			return True
		else:
			self.ResetBoard()
			return False

	def TryHorzMove(self,moveStartIndex,numberSpaces):
		#print("horz move starting at "+str(moveStartIndex) +" move " + str(numberSpaces)+" spaces")
		for i in range(int(numberSpaces)):
			numbers = ['1','2','3','4','5','6','7','8','9']
			if self.AllSquares[moveStartIndex+i].v == '_' or self.AllSquares[moveStartIndex+i].v == '.':
				self.AllSquares[moveStartIndex+i].SetV('-')

			elif self.AllSquares[moveStartIndex+i].v in numbers:
				if self.AllSquares[moveStartIndex+i].v == numberSpaces:
					pass
				else:
					return False
			else:
				return False

		return True

	def TryVertMove(self,moveStartIndex,numberSpaces):
		#print("vert move starting at "+str(moveStartIndex) +" move " + str(numberSpaces)+" spaces")
		for i in range(int(numberSpaces)):
			numbers = ['1','2','3','4','5','6','7','8','9']
			if self.AllSquares[moveStartIndex+(i*self.width)].v == '_' or self.AllSquares[moveStartIndex+(i*self.width)].v == '.':
				self.AllSquares[moveStartIndex+(i*self.width)].SetV('|')

			elif self.AllSquares[moveStartIndex+(i*self.width)].v in numbers:
			 	if self.AllSquares[moveStartIndex+(i*self.width)].v == numberSpaces:
			 		pass
			 	else:
			 		return False

			else:
				return False

		return True

	def IterateMoveIndexes(self,moveIndexes):
		impossible = True
		for i in range (len(moveIndexes)):
			if moveIndexes[i] == self.MoveAmmount(i):
				pass
			else:
				impossible=False
				break

		if impossible == True:
			print("puzzle is impossible")
			self.ResetBoard()
			moveIndexes = [0]*len(moveIndexes)
		else:
			for i in range(len(moveIndexes)-1,-1,-1):
				if moveIndexes[i] >= self.MoveAmmount(i):
					moveIndexes[i]=1
				else:
					moveIndexes[i]+=1
					break
		return moveIndexes

	def ResetBoard(self):
		#print("Reset Gameboard")
		i=0
		for y in range(self.height):
			for x in range(self.width):
				if self.puzzleString[y][x]=='.':
					self.AllSquares[i].SetV('.')
				i+=1

		for square in self.AllSquares:
			if square.v == '-' or square.v =='|':
				square.SetV('_')

	def MoveAmmount(self,index):
		return len(self.AllNumbers[index].moves)



if __name__ == "__main__":
	"""
	puzzle = [	".__4_.._4",
				"4...5____",
				"._3.__5__",
				".__.__2._",
				"5_.._5.._",
				"___4...4_",
				"___2..3._",
				"___.3..3_",
				"_______.5",
				"_.__4__4.",
				"_38._5___",
				"_..2.___3",
				"____3.3.."]
	"""

	#GameBoard = Puzzle(['0'])
	#GameBoard.PrintPuzzle()
	#GameBoard.SolvePuzzle()