# Survival Game 2D — Desafio Individual Unity

Projeto acadêmico de um jogo de sobrevivência 2D desenvolvido na Unity (C#) para a disciplina de Jogos Digitais. O objetivo do jogador é sobreviver ao maior número de ondas progressivas de inimigos utilizando movimentação, combate e estratégias de posicionamento.

---

## 🎮 Mecânicas e Funcionalidades

### **Jogador (Player)**
* **Movimentação:** Andar, Pulo duplo e Dash com tempo de recarga.
* **Sistema de Mira Multidirecional:** Disparos na horizontal, vertical e diagonal com ajuste dinâmico de animação e ponto de origem.
* **Combate com Minas:** Posicionamento de minas terrestres que detonam por área de efeito.
* **Game Loop Instantâneo:** Ao sofrer dano, a cena é recarregada imediatamente.

### **Inimigos & Dificuldade**
* **Inimigo Terrestre (Físico):** Patrulha e persegue o jogador afetado pela gravidade e colisão de plataformas.
* **Inimigo Fantasma (Atravessa Obstáculos):** Flutua livremente na direção do jogador ignorando estruturas do cenário.
* **Spawner Progressivo:** Gerador automático de inimigos que reduz o intervalo entre ondas com o tempo, aumentando o desafio continuamente.
---


