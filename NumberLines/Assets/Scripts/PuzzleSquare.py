
class Square():
	def __init__(self,index,x,y,v):
		self.index=index
		self.x = x
		self.y = y
		self.v = v
		self.completed=False
		self.filled=False
		self.moves=[]

		numbers = ['1','2','3','4','5','6','7','8','9']
		if self.v in numbers:
			self.filled=True

	def SetV(self,v):
		self.v=v
