//UNIT TEST
//TODO ReadServer의 스트레스 UnitTest가 필요함 수 많은 데이터를 보낼 경우 부하를 견딜 수 있는지 검사해야함
//TODO IDbAction이 실제로 작동되는지 검사가 필요함(테스트 코드)

//SERVER
//TODO WRITE 서버에서 수정 시 에러에 대비해서 트랜잭션 코드를 만든다. 
//TODO PING PONG을 read/write 서버용으로 command를 하나씩 만들어서 1분씩 서버가 클라이언트를 검사하게 함

//CLIENT
//TODO 서버에 접속할 때 Dialog로 접속 경과를 알려주는 프로그레스 다이얼로그를 만들어야함 실패 시 (닫기, 재접속을 행하는 버튼도 같이) 
//TODO 서버에 접근하는 모든 행위에 대해서 사전에 Connected, Ping 검사를 하고 아닐 경우 접속 불가를 알려주는 다이얼로그를 만들어야함 (닫기, 재접속)

//STUDY
//TODO Invoke, BeginInvoke 차이점 알기