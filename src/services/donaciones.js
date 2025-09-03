// Importe de rutas para apis, funciones y utilidades
import {
    DONACIONES_API,
    DONACIONES_LISTAR_API,
    DONACIONES_LISTAR_RUBRO_API,
    DONACIONES_GET_BY_ID_API,
    DONACIONES_TOTAL_RUBRO_API,
    DONACIONES_REPORTE_PROYECTO_API,
    DONACIONES_TOP_DONANTES_API
} from '../config/settings.js';
import { showSuccess, showError, showAlert, showConfirmation } from '../utils/sweetAlert.js';
import { getData, deleteData, postData, putData } from '../data/methods.js';

// Obtener todas las donaciones y mostrar
const obtenerTodasLasDonaciones = async () => {
    try {
        const response = await getData(DONACIONES_LISTAR_API);

        if (response && response.data) {
            console.log('Donaciones obtenidas:', response.data);

            // Inicializar DataTable con los datos obtenidos
            $('#tablaDonaciones').DataTable({
                destroy: true,
                data: response.data,
                columns: [
                    { data: "id" },
                    { 
                        data: "idRubro",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "monto",
                        render: function(data) {
                            // Formatear monto como moneda
                            return data ? `Q ${parseFloat(data).toFixed(2)}` : 'Q 0.00';
                        }
                    },
                    { 
                        data: "fechaDonacion",
                        render: function(data) {
                            // Formatear fecha para mejor visualización
                            return data ? new Date(data).toLocaleDateString('es-GT') : 'N/A';
                        }
                    },
                    { 
                        data: "nombreDonante",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "codigoRubro",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "nombreRubro",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "codigoProyecto",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    { 
                        data: "nombreProyecto",
                        render: function(data) {
                            return data || 'N/A';
                        }
                    },
                    {
                        data: "active",
                        render: function(data) {
                            // Mostrar estado activo/inactivo
                            return data === 1 ? 
                                '<span class="badge bg-success">Activo</span>' : 
                                '<span class="badge bg-danger">Inactivo</span>';
                        }
                    },
                    {
                        data: null,
                        render: function (data, type, row) {
                            return `
                                <div class="btn-group" role="group">
                                    <button onclick="editarDonacion(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-warning btn-sm">Editar</button>
                                    <button onclick="eliminarDonacion(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-danger btn-sm">Eliminar</button>
                                </div>
                            `;
                        }
                    }
                ],
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'copy',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-copy"></i> Copiar'
                    },
                    {
                        extend: 'excel',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-file-excel"></i> Excel'
                    },
                    {
                        extend: 'pdf',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-file-pdf"></i> PDF'
                    },
                    {
                        extend: 'print',
                        className: 'btn btn-secondary',
                        text: '<i class="fas fa-print"></i> Imprimir'
                    }
                ],
                language: {
                    url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
                },
                responsive: true,
                order: [[0, 'desc']], // Ordenar por ID descendente por defecto
                pageLength: 10, // Mostrar 10 registros por página
                lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]]
            });
        } else {
            showAlert("No se encontraron donaciones.", "warning");
            // Inicializar DataTable vacío para que se muestren los botones de exportación
            $('#tablaDonaciones').DataTable({
                dom: 'Bfrtip',
                buttons: [
                    {
                        extend: 'copy',
                        className: 'btn btn-secondary'
                    },
                    {
                        extend: 'excel',
                        className: 'btn btn-secondary'
                    },
                    {
                        extend: 'pdf',
                        className: 'btn btn-secondary'
                    },
                    {
                        extend: 'print',
                        className: 'btn btn-secondary'
                    }
                ],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
                }
            });
        }
    } catch (error) {
        showError(error.message, "danger");
        throw error;
    }
};

window.onload = async () => {
    await obtenerTodasLasDonaciones();
};