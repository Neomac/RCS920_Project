MODULE MainModule
	CONST robtarget p10:=[[54.11,-886.56,638.03],[0.0468888,-0.383027,-0.922388,0.0171113],[-1,-1,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]];
	CONST robtarget p20:=[[40.84,-1186.05,637.99],[0.0469084,-0.383036,-0.922383,0.0170943],[-1,-1,-1,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]];
	PROC main()
		MoveL p10, v1000, fine, tool0;
		MoveL p20, v1000, fine, tool0;
	ENDPROC
ENDMODULE