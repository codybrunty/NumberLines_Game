
class WriteOutPuzzle():

	def __init__(self,newLines):

		writeLine = ""
		for i in range(len(newLines)):
			writeLine += newLines[i]+","
			


		file = open("D:/progamFiles2/Repos/GitHub/NumberLines_Game/NumberLines/Assets/Puzzles/puzzles.txt", "a")  # append mode 


		file.write(writeLine+"\n") 
		file.close() 



if __name__ == "__main__":
	pass
	#WriteOutPuzzle("asdf")