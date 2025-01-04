<?php
if ($_SERVER["REQUEST_METHOD"] === "POST") {
    
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

    //if (isset($_POST["PlayerID"])) {
    //    $id = $_POST["PlayerID"];
    //} else {
    //    $id = "null";
    //}
    
    if (isset($_POST["position_X"])) {
        $px = $_POST["position_X"];
    } else {
        $px = "null";
    }
    
    if (isset($_POST["position_Y"])) {
        $py = $_POST["position_Y"];
    } else {
        $py = "null";
    }

    if (isset($_POST["position_Z"])) {
        $pz = $_POST["position_Z"];
    } else {
        $pz = "null";
    }

    // Consulta a la base de datos
    $sql = "INSERT INTO Death(`x`, `y`, `z`) VALUES ('$px', '$py', '$pz')";
    $result = $conn->query($sql);

    //Return to Unity
    echo $result;

    // Cerrar la conexión cuando hayas terminado
    $conn->close();

}
else {
    echo "nullData";
}
?>