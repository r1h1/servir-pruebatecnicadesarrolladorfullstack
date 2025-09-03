// Para GET y DELETE sin Token
async function fetchData(url, method, headers = {}) {
    const response = await fetch(url, {
        method,
        mode: "cors",
        headers: {
            "Content-Type": "application/json",
            "Cache-Control": "no-cache, no-store, must-revalidate",
            ...headers
        },
    });

    if (!response.ok) {
        let errorMessage = `Error ${response.status}: ${response.statusText}`;

        try {
            const errorData = await response.json();
            errorMessage = errorData.message || errorMessage;
        } catch {
            errorMessage = await response.text();
        }

        throw new Error(errorMessage);
    }

    // Si la respuesta es 204 No Content o DELETE, no intentamos parsear JSON
    if (response.status === 204) {
        return null;
    }

    return await response.json();
}

// Para POST y PUT sin Token
async function sendData(url, method, data, headers = {}) {
    const response = await fetch(url, {
        method,
        mode: "cors",
        headers: {
            "Content-Type": "application/json",
            "Cache-Control": "no-cache, no-store, must-revalidate",
            ...headers
        },
        body: JSON.stringify(data),
    });

    if (!response.ok) {
        let errorMessage = "Error en la petición.";

        try {
            const errorData = await response.json();
            errorMessage = errorData.message || errorMessage;
        } catch {
            errorMessage = await response.text();
        }

        throw new Error(errorMessage);
    }

    return await response.json();
}

// Función específica para GET (simplificada)
async function getData(url, headers = {}) {
    return fetchData(url, 'GET', headers);
}

// Función específica para DELETE (simplificada)
async function deleteData(url, headers = {}) {
    return fetchData(url, 'DELETE', headers);
}

// Función específica para POST (simplificada)
async function postData(url, data, headers = {}) {
    return sendData(url, 'POST', data, headers);
}

// Función específica para PUT (simplificada)
async function putData(url, data, headers = {}) {
    return sendData(url, 'PUT', data, headers);
}


export { fetchData, sendData, getData, deleteData, postData, putData };