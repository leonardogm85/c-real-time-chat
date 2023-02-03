const connection = new signalR
    .HubConnectionBuilder()
    .withUrl("/ChatHub")
    .build();

const startConnection = async () => {
    await connection
        .start()
        .then(() => {
            console.info('Started!');
        })
        .catch((err) => {
            console.error(err);
            setTimeout(startConnection, 5000);
        });
};

connection.onclose(startConnection);

