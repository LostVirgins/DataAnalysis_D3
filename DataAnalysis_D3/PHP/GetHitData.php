<?php

	 // Database config
    $servername = 'localhost';
    $dbname = 'jannl';
    $username = 'jannl';
    $password = '47752038j';

    // Create a connection to the database
    $conn = new mysqli($servername, $username, $password, $dbname);
    
    if($conn->connect_error) {
        die("Connection failed: " . $conn->connect_error);
    }

    //echo "Connection Success <br><br>";

    $sql = "SELECT x, y, z FROM Hit";
    $result = $conn->query($sql);
    
    if ($result->num_rows > 0) {
      // output data of each row
      while($row = $result->fetch_assoc()) {
        //echo "x" . $row["x"]. "/" . "y" . $row["y"]. "/" . "z" . $row["z"]. "/";
        echo $row["x"]. "/" . $row["y"]. "/" . $row["z"]. "/";
      }
    } else {
      echo "0 results";
    }
    $conn->close();

?>