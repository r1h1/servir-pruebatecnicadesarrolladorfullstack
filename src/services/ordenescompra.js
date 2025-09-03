// Importe de rutas para apis, funciones y utilidades
import {
    ORDENES_COMPRA_API,
    ORDENES_COMPRA_GET_BY_ID_API,
    ORDENES_COMPRA_RUBRO_API,
    ORDENES_COMPRA_COMPLETAS_API,
    ORDENES_COMPRA_TOTAL_RUBRO_API,
    ORDENES_COMPRA_RANGO_FECHAS_API,
    ORDENES_COMPRA_REPORTE_PROYECTO_API,
    ORDENES_COMPRA_BALANCE_RUBRO_API,
    ORDENES_COMPRA_INACTIVAR_RUBRO_API
} from '../config/settings.js';
import { showSuccess, showError, showAlert, showConfirmation } from '../utils/sweetAlert.js';
import { getData, deleteData, postData, putData } from '../data/methods.js';

// Obtener todas las órdenes de compra y mostrar
const obtenerTodasLasOrdenesDeCompra = async () => {
    try {
        const response = await getData(ORDENES_COMPRA_COMPLETAS_API);

        if (response && response.data) {

            // Inicializar DataTable con los datos obtenidos
            $('#tablaOrdenesCompra').DataTable({
                destroy: true,
                data: response.data,
                columns: [
                    { data: "id" },
                    { 
                        data: "monto",
                        render: function(data) {
                            // Formatear monto como moneda
                            return data ? `Q ${parseFloat(data).toFixed(2)}` : 'Q 0.00';
                        }
                    },
                    { 
                        data: "fechaOrden",
                        render: function(data) {
                            // Formatear fecha para mejor visualización
                            return data ? new Date(data).toLocaleDateString('es-GT') : 'N/A';
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
                            return data === true || data === 1 ? 
                                '<span class="badge bg-success">Activo</span>' : 
                                '<span class="badge bg-danger">Inactivo</span>';
                        }
                    },
                    {
                        data: null,
                        render: function (data, type, row) {
                            return `
                                <div class="btn-group" role="group">
                                    <button onclick="editarOrdenCompra(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-warning btn-sm">Editar</button>
                                    <button onclick="eliminarOrdenCompra(${JSON.stringify(row).replace(/'/g, "&#39;").replace(/"/g, "&quot;")})" class="btn btn-danger btn-sm">Eliminar</button>
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
                lengthMenu: [[5, 10, 25, 50, -1], [5, 10, 25, 50, "Todos"]],
                columnDefs: [
                    { width: "5%", targets: 0 },  // ID
                    { width: "10%", targets: 1 }, // Monto
                    { width: "10%", targets: 2 }, // Fecha Orden
                    { width: "10%", targets: 3 }, // Código Rubro
                    { width: "15%", targets: 4 }, // Nombre Rubro
                    { width: "10%", targets: 5 }, // Código Proyecto
                    { width: "15%", targets: 6 }, // Nombre Proyecto
                    { width: "5%", targets: 7 },  // Activo
                    { width: "15%", targets: 8 }  // Acciones
                ]
            });
        } else {
            showAlert("No se encontraron órdenes de compra.", "warning");
            // Inicializar DataTable vacío para que se muestren los botones de exportación
            $('#tablaOrdenesCompra').DataTable({
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
    await obtenerTodasLasOrdenesDeCompra();
};