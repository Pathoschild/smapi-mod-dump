#ifndef CONFIG_HPP
#define CONFIG_HPP

#include <map>
#include <string>

class Config
{
    public:
        Config();

        bool loadFromFile( const std::string& path );
        bool saveToFile( const std::string& path ) const;

        void setContentFolder( const std::string& path );
        std::string getContentFolder() const;

        void setExportFolder( const std::string& path );
        std::string getExportFolder() const;

        void setDataFolder( const std::string& path );
        std::string getDataFolder() const;

        void setExtractedSounds( const std::string& theExtractedSounds );
        std::string getExtractedSounds() const;

        void reset();

    private:
        std::string content;
        std::string exportFolder;
        std::string dataFolder;
        std::string extractedSounds;

        friend class Ui; // TODO: A specific ui window
};

#endif // CONFIG_HPP
