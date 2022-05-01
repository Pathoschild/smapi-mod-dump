if __name__ == '__main__':
    import sys
    import winsound
    import os
    current_dir = os.path.dirname(os.path.abspath(__file__))
    winsound.PlaySound(os.path.join(current_dir, '..', 'assets', 'ready.wav'), winsound.SND_FILENAME)