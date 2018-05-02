CSC = csc
SRC = $(subst /,\,$(shell gfind src -name \*.cs))

PRJC   = eveditor
#TARGET = exe
TARGET = winexe

default:
	$(CSC) /out:bin\$(PRJC).exe /target:$(TARGET) $(SRC)

run:
	bin\$(PRJC).exe
