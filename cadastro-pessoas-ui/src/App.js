import React, { useState, useEffect } from 'react';
import './App.css';
import Login from './components/Login';
import Dashboard from './components/Dashboard';
import PessoaForm from './components/PessoaForm';
import PessoasList from './components/PessoasList';
import PessoaFilter from './components/PessoaFilter';
import { Container, Paper, Typography, Button, Box } from '@mui/material';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import LogoutIcon from '@mui/icons-material/Logout';

function App() {
  const [token, setToken] = useState(null);
  const [view, setView] = useState('dashboard');
  const [pessoas, setPessoas] = useState([]);
  const [pessoaParaEditar, setPessoaParaEditar] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);

  const getAuthHeaders = () => ({
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  });

  const fetchPessoas = (filters = {}) => {
    if (!token) return;
    setIsLoading(true);
    setHasSearched(true);
    const params = new URLSearchParams();
    if (filters.nome) params.append('nome', filters.nome);
    if (filters.cpf) params.append('cpf', filters.cpf);
    
    fetch(`http://localhost:5182/api/v1/Pessoas?${params.toString()}`, { headers: getAuthHeaders() })
      .then(res => res.json())
      .then(data => { setPessoas(data); setIsLoading(false); })
      .catch(() => { setIsLoading(false); toast.error("Falha ao buscar dados da API."); });
  };
  
  const handleSave = (pessoa) => {
    const isEditing = !!pessoa.id;
    const url = isEditing ? `http://localhost:5182/api/v1/Pessoas/${pessoa.id}` : 'http://localhost:5182/api/v1/Pessoas';
    const method = isEditing ? 'PUT' : 'POST';

    fetch(url, { method, headers: getAuthHeaders(), body: JSON.stringify(pessoa) })
      .then(async response => {
        if (response.ok) {
          toast.success(`Pessoa ${isEditing ? 'atualizada' : 'cadastrada'}!`);
          setPessoaParaEditar(null);
          setView('dashboard');
        } else {
          const errorMessage = await response.text();
          toast.error(errorMessage || 'Erro ao salvar. Verifique os dados.');
        }
      });
  };
  
  const handleDelete = (id) => {
    if (window.confirm('Tem certeza?')) {
      fetch(`http://localhost:5182/api/v1/Pessoas/${id}`, { method: 'DELETE', headers: getAuthHeaders() })
        .then(res => {
          if (res.ok) {
            toast.success('Pessoa excluída!');
            setPessoas(pessoas.filter(p => p.id !== id));
          } else {
            toast.error('Erro ao excluir.');
          }
        });
    }
  };

  // --- INÍCIO DA CORREÇÃO ---
  const handleLogout = () => {
    setToken(null);
    setView('dashboard');
    setPessoas([]);       // Limpa a lista de pessoas
    setHasSearched(false); // Reseta o estado da busca
  };

  const handleNavigate = (targetView) => {
    setView(targetView);
    setPessoas([]);       // Limpa a lista de pessoas ao navegar
    setHasSearched(false); // Reseta o estado da busca
  };
  // --- FIM DA CORREÇÃO ---

  if (!token) {
    return (
        <>
            <ToastContainer position="top-right" autoClose={3000} />
            <Login onLoginSuccess={setToken} />
        </>
    );
  }

  return (
    <>
      <ToastContainer position="top-right" autoClose={3000} />
      <Box sx={{ position: 'absolute', top: 16, right: 16, zIndex: 1200 }}>
        <Button 
            variant="contained" 
            color="error" 
            onClick={handleLogout}
            startIcon={<LogoutIcon />}
        >
          Sair
        </Button>
      </Box>
      <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
        
        {view !== 'dashboard' && (
          // Usa a nova função de navegação para garantir a limpeza
          <Button startIcon={<ArrowBackIcon />} onClick={() => handleNavigate('dashboard')} sx={{ mb: 2 }}>
            Voltar ao Menu
          </Button>
        )}

        {/* Usa a nova função de navegação */}
        {view === 'dashboard' && <Dashboard onNavigate={handleNavigate} />}
        
        {view === 'cadastro' && (
          <Paper elevation={3} sx={{ p: 3, mb: 4 }}>
            <PessoaForm onSave={handleSave} pessoaParaEditar={pessoaParaEditar} onCancelEdit={() => handleNavigate('dashboard')} />
          </Paper>
        )}
        
        {view === 'consulta' && (
          <>
            <PessoaFilter onSearch={fetchPessoas} />
            {hasSearched && (
              <PessoasList 
                pessoas={pessoas} 
                onDelete={handleDelete} 
                onEdit={(pessoa) => { setPessoaParaEditar(pessoa); setView('cadastro'); }} 
                isLoading={isLoading} 
              />
            )}
          </>
        )}
      </Container>
    </>
  );
}

export default App;