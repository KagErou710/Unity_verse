package main

import (
	"fmt"
	"net"

	"strconv"
	"strings"
	"sync"
	"time"
)

var wg sync.WaitGroup
var UdpClientsList = make(map[string]*udpClientInfo)
var TcpClientsList = make(map[net.TCPConn]tcpClientInfo)

func sendUdpResponse(conn *net.UDPConn, addr *net.UDPAddr, message []byte) {

	_, err := conn.WriteToUDP(message, addr)
	if err != nil {
		fmt.Printf("Couldn't send response %v", err)
	}
}

type udpClientInfo struct {
	playerEP net.UDPAddr
	playerID string
}

type tcpClientInfo struct {
	playerID   string
	toDoList   []string
	stillAlive bool
}

type TempPlayerData struct {
	p_ID string
	p_x  string
	p_y  string
	p_z  string
}

func addUDPClientInfo(EP *net.UDPAddr, ID string) *udpClientInfo {
	client := udpClientInfo{playerEP: *EP, playerID: ID}
	return &client
}

func UdpAddPlayerList(conn *net.UDPConn, client udpClientInfo) {
	if _, exists := UdpClientsList[client.playerID]; !exists {
		packPlayerInfo(conn, client)
		UdpClientsList[client.playerID] = &client
		addMessage := "3," + client.playerID + ","
		sendAllUdpSubscriber([]byte(addMessage), conn, client)
	}
}

func packPlayerInfo(conn *net.UDPConn, client udpClientInfo) {

	for k := range UdpClientsList {
		tempClient := UdpClientsList[k]
		packMessage := "4," + tempClient.playerID + ",0,0,0;"
		sendUdpResponse(conn, &client.playerEP, []byte(packMessage))
	}
}

func tcpPackplayerInfo(conn *net.TCPConn) {
	for _, e := range TcpClientsList {
		packMessage := "4," + e.playerID + ",0,0,0;"
		conn.Write([]byte(packMessage))
	}
}

func sendAllUdpSubscriber(message []byte, conn *net.UDPConn, client udpClientInfo) {
	for k := range UdpClientsList {
		if k != client.playerID {
			tempClient := UdpClientsList[k].playerEP
			sendUdpResponse(conn, &tempClient, message)
		}
	}
}

func sendAllTcpSubscriber(message string, playerID string) {
	for k, e := range TcpClientsList {
		if e.playerID != playerID {
			entry, _ := TcpClientsList[k]
			entry.toDoList = append(entry.toDoList, message)
			TcpClientsList[k] = entry
		}
	}
}

func checkTCP(conn *net.TCPConn) bool {
	tempBuf := make([]byte, 500)
	_, err := conn.Read(tempBuf)
	if err != nil {
		conn.Close()
		fmt.Println(err)
		return false
	}

	return true
}

func youStillAlive() {
	for _ = range time.Tick(time.Second * 3) {
		fmt.Println(len(TcpClientsList))
		for conn, player := range TcpClientsList {
			if player.stillAlive == true {
				player.stillAlive = false
				TcpClientsList[conn] = player
				aliveMessage := "5,;"
				conn.Write([]byte(aliveMessage))
			} else {
				askDelete := "6," + player.playerID + ",;"
				sendAllTcpSubscriber(askDelete, player.playerID)
				delete(TcpClientsList, conn)
			}
		}
	}
}

func tcpManager(listener net.TCPListener) {
	for {
		conn, _ := listener.AcceptTCP()
		var myID string
		go func(conn *net.TCPConn, myID string) {
			tcpCommunicator(conn, myID)
		}(conn, myID)
	}
}

func tcpCommunicator(conn *net.TCPConn, myID string) {
	defer conn.Close()
	for {
		buf := make([]byte, 500)
		messageByte, _ := conn.Read(buf)
		message := string(buf[:messageByte])
		recvTcpTexts := strings.Split(message, ",")

		switch recvTcpTexts[0] {
		case "9":
			for i := 1; i < 100; i++ {
				TcpClientsList[*conn] = tcpClientInfo{strconv.Itoa(i), []string{}, true}
				message := "3," + strconv.Itoa(i) + ",;"
				sendAllTcpSubscriber(message, recvTcpTexts[1])
			}
		case "3":
			tcpPackplayerInfo(conn)
			TcpClientsList[*conn] = tcpClientInfo{recvTcpTexts[1], []string{}, true}
			sendAllTcpSubscriber(message, recvTcpTexts[1])
			myID = recvTcpTexts[1]
		case "5":
			for conn, v := range TcpClientsList {
				if v.playerID == recvTcpTexts[1] {
					v.stillAlive = true
					TcpClientsList[conn] = v
				}
			}
		default:
			if len(recvTcpTexts) > 1 {
				sendAllTcpSubscriber(message, recvTcpTexts[1])
			}
		}
	}
}

func sendTCP() {
	for _ = range time.Tick(time.Second * 3) {
		for conn := range TcpClientsList {
			if len(TcpClientsList[conn].toDoList) > 0 {
				for k := range TcpClientsList[conn].toDoList {
					messageFromList := TcpClientsList[conn].toDoList[k]
					conn.Write([]byte(messageFromList))
				}
				entry, _ := TcpClientsList[conn]
				entry.toDoList = nil
				TcpClientsList[conn] = entry
			}
		}
	}
}

func main() {

	addr := net.UDPAddr{
		Port: 9357,
		IP:   net.ParseIP("127.0.0.1"),
	}
	ser, err := net.ListenUDP("udp", &addr)
	if err != nil {
		fmt.Printf("Some error %v\n", err)
		return
	}

	tcpAddr, _ := net.ResolveTCPAddr("tcp", ":8052")
	listener, _ := net.ListenTCP("tcp", tcpAddr)

	defer listener.Close()

	go func(listener net.TCPListener) {
		tcpManager(listener)
	}(*listener)

	go func() {
		sendTCP()
	}()

	go func() {
		youStillAlive()
	}()

	for {

		p := make([]byte, 500)
		_, remoteaddr, err := ser.ReadFromUDP(p)
		recvTexts := strings.Split(string(p), ",")

		tempClient := addUDPClientInfo(remoteaddr, recvTexts[1])
		UdpAddPlayerList(ser, *tempClient)
		if err != nil {
			fmt.Printf("Some error  %v", err)
			continue
		}
		sendAllUdpSubscriber(p, ser, *tempClient)
	}
}
