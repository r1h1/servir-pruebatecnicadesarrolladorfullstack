// Importe de rutas para apis, funciones y utilidades
import {
    ORDENES_COMPRA_BALANCE_RUBRO_API
} from '../config/settings.js';
import { showAlert, showError } from '../utils/sweetAlert.js';
import { getData } from '../data/methods.js';

// Obtener balance de un rubro específico y mostrar en tabla
const obtenerBalanceRubro = async (idRubro) => {
    try {
        if (!idRubro || idRubro <= 0) {
            showAlert("Por favor, introduce un ID de rubro válido.", "warning");
            return;
        }

        const response = await getData(ORDENES_COMPRA_BALANCE_RUBRO_API(idRubro));

        if (response && response.success === 1 && response.data) {
            // Crear array con el resultado para DataTable
            const balanceData = [{
                "id": idRubro,
                "idRubro": idRubro,
                "totalDonaciones": response.data.totalDonaciones,
                "totalOrdenesCompra": response.data.totalOrdenesCompra,
                "balance": response.data.balance
            }];

            // Inicializar DataTable con los datos obtenidos
            $('#tablaBalances').DataTable({
                destroy: true,
                data: balanceData,
                columns: [
                    { data: "id" },
                    { data: "idRubro" },
                    { 
                        data: "totalDonaciones",
                        render: function(data) {
                            return data ? `Q ${parseFloat(data).toFixed(2)}` : 'Q 0.00';
                        }
                    },
                    { 
                        data: "totalOrdenesCompra",
                        render: function(data) {
                            return data ? `Q ${parseFloat(data).toFixed(2)}` : 'Q 0.00';
                        }
                    },
                    { 
                        data: "balance",
                        render: function(data) {
                            const balanceClass = data >= 0 ? 'text-success' : 'text-danger';
                            return `<span class="${balanceClass}">Q ${parseFloat(data || 0).toFixed(2)}</span>`;
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
                    { width: "10%", targets: 0 },  // ID
                    { width: "10%", targets: 1 },  // ID Rubro
                    { width: "20%", targets: 2 },  // Total Donaciones
                    { width: "20%", targets: 3 },  // Total Órdenes Compra
                    { width: "20%", targets: 4 }   // Balance
                ]
            });
        } else {
            showAlert(response?.message || "No se pudo obtener el balance del rubro.", "warning");
            // Inicializar DataTable vacío
            $('#tablaBalances').DataTable({
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

// Configurar event listeners
const configurarEventListeners = () => {
    // Event listener para el input (al presionar Enter)
    document.getElementById('idRubroInput').addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            const idRubro = parseInt(this.value);
            obtenerBalanceRubro(idRubro);
        }
    });
};

// Inicializar la página
window.onload = async () => {
    // Inicializar DataTable vacío al cargar la página
    $('#tablaBalances').DataTable({
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
    
    // Configurar event listeners
    configurarEventListeners();
};